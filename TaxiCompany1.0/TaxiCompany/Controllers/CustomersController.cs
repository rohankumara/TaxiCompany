using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using TaxiCompany.Authorization;
using TaxiCompany.Data;
using TaxiCompany.Models;

namespace TaxiCompany.Controllers
{
    public class CustomersController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IAuthorizationService _authorizationService;
        private readonly UserManager<ApplicationUser> _userManager;

        public CustomersController(ApplicationDbContext context, IAuthorizationService authorizationService,
                                    UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
            _authorizationService = authorizationService;
        }

        // GET: Customers
        public async Task<IActionResult> Index(string sortOder, string searchString, string currentfilter, int? page)
        {
            var cus = from c in _context.Customer
                      select c;

            var isAuthorized = User.IsInRole(Constants.TaxiOfficeAdministratorsRole);
            var currentUserId = _userManager.GetUserId(User);
            // Only approved contacts are shown UNLESS you're authorized to see them
            // or you are the owner.
            if (!isAuthorized)
            {
                cus = cus.Where(c => c.Status == Customer.CustomerStatus.Approved || c.OwnerID == currentUserId);
            }

            var applicationDbContext = _context.Customer.Include(c => c.Branch);

            ViewData["NameSortParm"] = String.IsNullOrEmpty(sortOder) ? "Name_desc" : "";

            if (searchString != null)
            {
                page = 1;
            }
            else
            {
                searchString = currentfilter;
            }

            ViewData["CurrentFilter"] = searchString;

            var customers = from c in _context.Customer
                            select c;

            if (!String.IsNullOrEmpty(searchString))
            {
                customers = customers.Where(s => s.Lastname.Contains(searchString) || s.Firstname.Contains(searchString));
            }

            switch (sortOder)
            {
                case "Name_desc":
                    customers = customers.OrderByDescending(c => c.Lastname);
                    break;
                default:
                    customers = customers.OrderBy(c => c.Lastname);
                    break;
            }

            int PageSize = 5;

            return View(await PaginatedList<Customer>.CreateAsync(customers.AsNoTracking(), page ?? 1, PageSize));
        }

        // GET: Customers/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var customer = await _context.Customer
                .Include(c => c.Branch)
                .SingleOrDefaultAsync(m => m.ID == id);
            if (customer == null)
            {
                return NotFound();
            }

            var isAuthorizedRead = await _authorizationService.AuthorizeAsync(User, customer, TaxiCompanyOperations.Read);

            var isAuthorizedApprove = await _authorizationService.AuthorizeAsync(User, customer, TaxiCompanyOperations.Approve);

            if (customer.Status != Customer.CustomerStatus.Approved && !isAuthorizedRead.Succeeded && !isAuthorizedApprove.Succeeded)
            {
                return new ChallengeResult();
            }

            return View(customer);
        }

        // GET: Customers/Create
        public IActionResult Create()
        {
            ViewData["BranchID"] = new SelectList(_context.Set<Branch>(), "ID", "City");
            return View();
        }

        // POST: Customers/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CustomerEditViewModel editModel)
        {
            if (!ModelState.IsValid)
            {
                return View(editModel);
            }
            var cus = ViewModel_to_model(new Customer(), editModel);
            cus.OwnerID = _userManager.GetUserId(User);
            var isAuthorized = await _authorizationService.AuthorizeAsync(User, cus, TaxiCompanyOperations.Create);

            if (!isAuthorized.Succeeded)
            {
                return new ChallengeResult();

            }

            ViewData["BranchID"] = new SelectList(_context.Branch, "ID", "City", editModel.BranchID);

            _context.Add(cus);
          
            await _context.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        // GET: Customers/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var customer = await _context.Customer.SingleOrDefaultAsync(m => m.ID == id);
            if (customer == null)
            {
                return NotFound();
            }
            var isAuthorized = await _authorizationService.AuthorizeAsync(User, customer, TaxiCompanyOperations.Update);

            if (!isAuthorized.Succeeded)
            {
                return new ChallengeResult();
            }

            var editModel = Model_to_viewModel(customer);

            ViewData["BranchID"] = new SelectList(_context.Branch, "ID", "City", customer.BranchID);
            return View(editModel);
        }

        // POST: Customers/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, CustomerEditViewModel editModel)
        {
            if (!ModelState.IsValid)
            {
                return View(editModel);
            }

            var customer = await _context.Customer.SingleOrDefaultAsync(m => m.ID == id);

            if (customer == null)
            {
                return NotFound();
            }

            var isAuthorized = await _authorizationService.AuthorizeAsync(User, customer, TaxiCompanyOperations.Update);

            if (!isAuthorized.Succeeded)
            {
                return new ChallengeResult();
            }

            customer = ViewModel_to_model(customer, editModel);

            if (customer.Status == Customer.CustomerStatus.Approved)
            {
                // If the update is updated after approval, 
                // and the user cannot approve set the status back to submitted
                var canApprove = await _authorizationService.AuthorizeAsync(User, customer, TaxiCompanyOperations.Approve);

                if (!canApprove.Succeeded) customer.Status = Customer.CustomerStatus.Submitted;
            }

            _context.Update(customer);
            await _context.SaveChangesAsync();

            ViewData["BranchID"] = new SelectList(_context.Branch, "ID", "City", customer.BranchID);
            return RedirectToAction("Index");
        }

        // GET: Customers/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var customer = await _context.Customer
                .Include(c => c.Branch)
                .SingleOrDefaultAsync(m => m.ID == id);
            if (customer == null)
            {
                return NotFound();
            }
            var isAuthorized = await _authorizationService.AuthorizeAsync(User, customer, TaxiCompanyOperations.Delete);
            if (!isAuthorized.Succeeded)
            {
                return new ChallengeResult();
            }
            return View(customer);
        }

        // POST: Customers/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var customer = await _context.Customer.SingleOrDefaultAsync(m => m.ID == id);
            var isAuthorized = await _authorizationService.AuthorizeAsync(User, customer, TaxiCompanyOperations.Delete);

            if (!isAuthorized.Succeeded)
            {
                return new ChallengeResult();
            }

            _context.Customer.Remove(customer);
            await _context.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SetStatus(int id, Customer.CustomerStatus status)
        {
            var customer = await _context.Customer.SingleOrDefaultAsync(m => m.ID == id);
            var customerOperation = (status == Customer.CustomerStatus.Approved) ? TaxiCompanyOperations.Approve : TaxiCompanyOperations.Reject;

            var isAuthorized = await _authorizationService.AuthorizeAsync(User, customer, customerOperation);
            if (!isAuthorized.Succeeded)
            {
                return new ChallengeResult();
            }

            customer.Status = status;
            _context.Customer.Update(customer);

            await _context.SaveChangesAsync();

            return RedirectToAction("Index");
        }

        private bool CustomerExists(int id)
        {
            return _context.Customer.Any(e => e.ID == id);
        }
        private Customer ViewModel_to_model(Customer customer, CustomerEditViewModel editModel)
        {
            customer.Lastname = editModel.Lastname;
            customer.Firstname = editModel.Firstname;
            customer.Career = editModel.Career;
            customer.gender = (Customer.Gender)editModel.gender;
            customer.Age = editModel.Age;
            customer.Homeaddress = editModel.Homeaddress;
            customer.Officeaddress = editModel.Officeaddress;
            customer.Telnumber = editModel.Telnumber;
            customer.Email = editModel.Email;
            customer.Branch = editModel.Branch;

            return customer;
        }

        private CustomerEditViewModel Model_to_viewModel(Customer customer)
        {
            var editModel = new CustomerEditViewModel();

            editModel.ID = customer.ID;
            editModel.Lastname = customer.Lastname;
            editModel.Firstname = customer.Firstname;
            editModel.Career = editModel.Career;
            editModel.gender = (CustomerEditViewModel.Gender)customer.gender;
            editModel.Age = customer.Age;
            editModel.Homeaddress = customer.Homeaddress;
            editModel.Officeaddress = customer.Officeaddress;
            editModel.Telnumber = customer.Telnumber;
            editModel.Email = customer.Email;
            editModel.Branch = customer.Branch;

            return editModel;

        }
    

    }
}
