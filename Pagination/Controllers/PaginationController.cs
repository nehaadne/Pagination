using Dapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Pagination.Models;
using System.Data.SqlClient;

namespace Pagination.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PaginationController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        public PaginationController(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        [HttpGet]
        public IActionResult Get(int currentPageNumber, int pageSize)
        {
            int maxPagSize = 50;
            pageSize = (pageSize > 0 && pageSize <= maxPagSize) ? pageSize : maxPagSize;

            int skip = (currentPageNumber - 1) * pageSize;
            int take = pageSize;

            string query = @"SELECT 
                            COUNT(*)
                            FROM Todo
 
                            SELECT  * FROM Todo
                            ORDER BY Id
                            OFFSET @Skip ROWS FETCH NEXT @Take ROWS ONLY";

            using (var connection = new SqlConnection(_configuration.GetConnectionString("SqlConnection")))
            {
                var reader = connection.QueryMultiple(query, new { Skip = skip, Take = take });

                int count = reader.Read<int>().FirstOrDefault();
                List<Todo> allTodos = reader.Read<Todo>().ToList();

                var result = new PagingResponse<List<Todo>>(allTodos, count, currentPageNumber, pageSize);
                return Ok(result);
            }
        }

    }
}
