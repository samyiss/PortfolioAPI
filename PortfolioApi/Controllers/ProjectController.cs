using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using PortfolioApi.Models;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace PortfolioApi.Controllers;

[Route("api/[controller]")]
[ApiController]
public class ProjectController(AppDbContext context) : ControllerBase
{
    private const string Value = """Projects done not found""";
    private readonly AppDbContext _context = context;

    // GET: api/projects
    [HttpGet]
    public async Task<IActionResult> Get()
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
                return NotFound(Value);
            }
 
            projects.ForEach(p =>
            {
                p.authors = p.authors_projects.Select(d => new Author
                {
                    author_id = d.authors.author_id,
                    name = d.authors.name,
                }).ToList();
                p.authors_projects.Clear();

                p.tags = p.Tags_projects.Select(d => new Tag
                {
                    tag_id = d.tags.tag_id,
                    name = d.tags.name,
                }).ToList();
                p.Tags_projects.Clear();
            });


            var json = JsonConvert.SerializeObject(projects , Formatting.Indented);

            // Specify the file path
            string downloadsFolder = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile) + "\\Downloads";
            string filePath = Path.Combine(downloadsFolder, $"ProjectsData.json");

            // Check if file exists, and delete if it does
            if (System.IO.File.Exists(filePath))
            {
                System.IO.File.Delete(filePath);
            }

            // Write the JSON to a file
            await System.IO.File.WriteAllTextAsync(filePath, json);

            // Return the file as a response
            byte[] fileBytes = await System.IO.File.ReadAllBytesAsync(filePath);
            File(fileBytes, "application/json", $"ProjectsData.json");

            return Ok("File uploaded successfully in your DOWNLOADS file");

        }
    }

    // GET api/projects/5
    [HttpGet("{id}")]
    public ActionResult<Project> Get(int id)
    {
        using (_context)
        {
            var project = _context.projects
            .Include(p => p.Tags_projects).ThenInclude(p => p.tags)
            .Include(p => p.authors_projects).ThenInclude(p => p.authors)
            .Include(p => p.pictures)
            .FirstOrDefault(p => p.project_id == id);

            if (project == null)
            {
                return NotFound("Project not found");
            }

            project.authors = project.authors_projects.Select(d => new Author
            {
                author_id = d.authors.author_id,
                name = d.authors.name,
            }).ToList();

            project.tags = project.Tags_projects.Select(d => new Tag
            {
                tag_id = d.tags.tag_id,
                name = d.tags.name,
            }).ToList();


            return project;
        }

    }

    // POST api/projects
    [HttpPost]
    public ActionResult<string> Post([FromBody] Project project)
    {
        if (project == null)
        {
            return BadRequest("Invalid project data");
        }
        try
        {
            using (_context)
            {
                var newProject = new Project
                {
                    name = project.name,
                    date_project = project.date_project,
                    description = project.description,
                    giturl = project.giturl,
                    pictures = project.pictures,
                    apiurl = project.apiurl,
                };

                _context.projects.Add(newProject);
                _context.SaveChanges();

                // Retrieve the newly generated project ID
                int projectId = newProject.project_id;

                // Insert pictures associated with the project
                if (project.pictures != null && project.pictures.Any())
                {
                    // Create a parameterized SQL command to insert pictures
                    var sql = "INSERT INTO pictures (picture, project_id) VALUES ";
                    var parameters = new List<object>();

                    foreach (var picture in project.pictures)
                    {
                        sql += "({0}, {1}),";
                        parameters.Add(picture.picture);
                        parameters.Add(projectId);
                    }

                    // Execute the SQL command to insert all records
                    _context.Database.ExecuteSqlRaw(sql.TrimEnd(','), parameters.ToArray());
                }

                if (project.authors != null && project.authors.Any())
                {
                    // Create a parameterized SQL command to insert pictures
                    var sql = "INSERT INTO authors_projects (project_id, author_id) VALUES ";
                    var parameters = new List<object>();

                    foreach (var author in project.authors)
                    {
                        sql += "({0}, {1}),";
                        parameters.Add(projectId);
                        parameters.Add(author.author_id);
                    }

                    // Execute the SQL command to insert all records
                    _context.Database.ExecuteSqlRaw(sql.TrimEnd(','), parameters.ToArray());
                }

                if (project.tags != null && project.tags.Any())
                {
                    // Create a parameterized SQL command to insert pictures
                    var sql = "INSERT INTO tags_projects (project_id, tag_id) VALUES ";
                    var parameters = new List<object>();

                    foreach (var tag in project.tags)
                    {
                        sql += "({0}, {1}),";
                        parameters.Add(projectId);
                        parameters.Add(tag.tag_id);
                    }

                    // Execute the SQL command to insert all records
                    _context.Database.ExecuteSqlRaw(sql.TrimEnd(','), parameters.ToArray());
                }


                return Ok("Project added successfully");
            }
        } catch (Npgsql.PostgresException ex)
        {
            // Check if the exception is due to a foreign key constraint violation
            if (ex.SqlState == "23503")
            {
                if (ex.Message.Contains("authors")) return BadRequest("the author you are trying to add is not found");
                else if (ex.Message.Contains("tags")) return BadRequest("the tags you are trying to add is not found");
            }

            // Return a general error message for other types of exceptions
            return StatusCode(500, "An error occurred while updating the project");
        }
    }

    // PUT api/projects/5
    [HttpPut("{id}")]
    public ActionResult<string> Put(int id, [FromBody] Project project)
    {
        using (_context)
        {
            // Retrieve the project entity from the database
            var projectToUpdate = _context.projects
                .Include(p => p.Tags_projects).ThenInclude(p => p.tags)
                .Include(p => p.authors_projects).ThenInclude(p => p.authors)
                .Include(p => p.pictures)
                .FirstOrDefault(p => p.project_id == id);

            if (projectToUpdate == null)
            {
                return NotFound("Project not found");
            }

            // Update project properties
            projectToUpdate.name = project.name;
            projectToUpdate.date_project = project.date_project;
            projectToUpdate.description = project.description;
            projectToUpdate.giturl = project.giturl;
            projectToUpdate.apiurl = project.apiurl;

            // Remove associated pictures
            _context.pictures.RemoveRange(projectToUpdate.pictures);

            // Add new pictures
            projectToUpdate.pictures = project.pictures;


            // Update project tags
            UpdateProjectTags(projectToUpdate, project.Tags_projects);

            // Update project authors
            UpdateProjectAuthors(projectToUpdate, project.authors_projects);

            // Save changes to the database
            _context.SaveChanges();

            return Ok("Project updated successfully");
        }
    }


    private void UpdateProjectTags(Project projectToUpdate, ICollection<Tags_Projects> newTags)
    {
        // Remove deleted tags
        foreach (var existingTag in projectToUpdate.Tags_projects.ToList())
        {
            if (!newTags.Any(t => t.tag_id == existingTag.tag_id))
            {
                projectToUpdate.Tags_projects.Remove(existingTag);
            }
        }

        // Add new tags
        foreach (var newTag in newTags)
        {
            if (!projectToUpdate.Tags_projects.Any(t => t.tag_id == newTag.tag_id))
            {
                projectToUpdate.Tags_projects.Add(newTag);
            }
        }
    }

    private void UpdateProjectAuthors(Project projectToUpdate, ICollection<Authors_Projects> newAuthors)
    {
        // Remove deleted authors
        foreach (var existingAuthor in projectToUpdate.authors_projects.ToList())
        {
            if (!newAuthors.Any(a => a.author_id == existingAuthor.author_id))
            {
                projectToUpdate.authors_projects.Remove(existingAuthor);
            }
        }

        // Add new authors
        foreach (var newAuthor in newAuthors)
        {
            if (!projectToUpdate.authors_projects.Any(a => a.author_id == newAuthor.author_id))
            {
                projectToUpdate.authors_projects.Add(newAuthor);
            }
        }
    }


    // DELETE api/projects/5
    [HttpDelete("{id}")]
    public ActionResult<string> Delete(int id)
    {
        using (_context)
        {
            // Find the project entity by its ID
            var project = _context.projects
                .Include(p => p.pictures) // Include related pictures
                .FirstOrDefault(p => p.project_id == id);

            if (project == null)
            {
                return NotFound("Project not found");
            }

            // Remove associated pictures
            _context.pictures.RemoveRange(project.pictures);


            var authorsProjectsToRemove = _context.authors_projects
            .Where(ap => ap.project_id == id)
            .ToList();

            _context.authors_projects.RemoveRange(authorsProjectsToRemove);

            var tagsProjectsToRemove = _context.tags_projects
            .Where(ap => ap.project_id == id)
            .ToList();

            _context.tags_projects.RemoveRange(tagsProjectsToRemove);

            // Remove the project from the context
            _context.projects.Remove(project);

            // Save changes to the database
            _context.SaveChanges();

            return Ok("Project deleted successfully");
        }
    }
}
