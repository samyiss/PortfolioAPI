using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PortfolioApi.Models;

namespace PortfolioApi.Controllers;

[Route("api/[controller]")]
[ApiController]
public class ProjectController(AppDbContext context) : ControllerBase
{

    private readonly AppDbContext _context = context;

    // GET: api/projects
    [HttpGet]
    public ActionResult<List<Project>> Get()
    {
        var response = new ApiResponse();

        using (_context)
        {
            var projects = _context.projects
            .Include(p => p.Tags_projects).ThenInclude(p => p.tags)
            .Include(p => p.authors_projects).ThenInclude(p => p.authors)
            .Include(p => p.pictures)
            .ToList();

            if(projects.Count == 0)
            {
                return NotFound("Project not found");
            }
 
            projects.ForEach(p =>
            {
                p.authors = p.authors_projects.Select(d => new Author
                {
                    author_id = d.authors.author_id,
                    name = d.authors.name,
                    giturl = d.authors.giturl
                }).ToList();

                p.tags = p.Tags_projects.Select(d => new Tag
                {
                    tag_id = d.tags.tag_id,
                    name = d.tags.name,
                }).ToList();
            });


            return projects;
        }
    }

    // GET api/projects/5
    [HttpGet("{id}")]
    public ActionResult<Project> Get(int id)
    {
        var project = _context.projects.Find(id);

        if (project == null)
        {
            return NotFound("Project not found");
        }

        return project;
    }

    // POST api/projects
    [HttpPost]
    public void Post([FromBody] string value)
    {
    }

    // PUT api/projects/5
    [HttpPut("{id}")]
    public void Put(int id, [FromBody] string value)
    {
    }

    // DELETE api/projects/5
    [HttpDelete("{id}")]
    public void Delete(int id)
    {
    }
}
