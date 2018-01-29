using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using thoughtless_eels.Data;
using thoughtless_eels.Models;


namespace thoughtless_eels.Controllers
{
    [Route("api/[controller]")]
    public class EmployeeController : Controller
    {
        private bool EmployeeExists(int employeeId)
        {
            return _context.Employee.Any(g => g.EmployeeId == employeeId);
        }
        private ApplicationDbContext _context;
        // Constructor method to create an instance of context to communicate with our database.
        public EmployeeController(ApplicationDbContext ctx)
        {
            _context = ctx;
        }

        // POST api/values
        [HttpPost]
        public IActionResult Post([FromBody]Employee employee)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _context.Employee.Add(employee);

            try
            {
                _context.SaveChanges();
            }
            catch (DbUpdateException)
            {
                if (EmployeeExists(employee.EmployeeId))
                {
                    return new StatusCodeResult(StatusCodes.Status409Conflict);
                }
                else
                {
                    throw;
                }
            }
            return CreatedAtRoute("GetSingleEmployee", new { id = employee.EmployeeId }, employee);
        }

        [HttpPost ("{id}/assignComputer", Name="assignComputer")]
        public IActionResult Post(int id, [FromBody]EmployeeComputer employeeComputer)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

			Employee employee = _context.Employee.Single(e => e.EmployeeId == id);
			Computer computer = _context.Computer.Single(c => c.ComputerId == employeeComputer.ComputerId);
            

            if (employee == null || computer == null)
            {
                return NotFound();
            }

            _context.EmployeeComputer.Add(employeeComputer);
            _context.SaveChanges();
            return CreatedAtRoute("GetSingleProduct", new { id = employeeComputer.EmployeeId}, employee);
        }

        [HttpPost ("{id}/assignTraining")]
        public IActionResult Post(int id, [FromBody]EmployeeTraining employeeTraining)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

			Employee employee = _context.Employee.Single(e => e.EmployeeId == id);
			TrainingProgram trainingProgram = _context.TrainingProgram.Single(tp => tp.TrainingProgramId == employeeTraining.TrainingProgramId);
            

            if (employee == null || trainingProgram == null)
            {
                return NotFound();
            }

            _context.EmployeeTraining.Add(employeeTraining);
            _context.SaveChanges();
            return CreatedAtRoute("GetSingleProduct", new { id = employeeTraining.EmployeeTrainingId}, employee);
        }

        [HttpGet]
        public IActionResult Get()
        {
            var employees = _context.Employee.ToList();
            if (employees == null)
            {
                return NotFound();
            }
            return Ok(employees);
        }

        // GET api/values/5
        [HttpGet("{id}", Name = "GetSingleEmployee")]
        public IActionResult Get(int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                Employee employee = _context.Employee.Single(g => g.EmployeeId == id);

                if (employee == null)
                {
                    return NotFound();
                }

                return Ok(employee);
            }
            catch (System.InvalidOperationException ex)
            {
                return NotFound();
            }
        }

        [HttpPut("{id}")]
        public IActionResult Put(int id, [FromBody]Employee employee)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != employee.EmployeeId)
            {
                return BadRequest();
            }
            _context.Employee.Update(employee);
            try
            {
                _context.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!EmployeeExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return new StatusCodeResult(StatusCodes.Status204NoContent);
        }
    }
}