using System.Threading.Tasks;
using VirtualHole.DB;
using VirtualHole.API.Models;

namespace VirtualHole.API.Controllers
{
	public class PagingFindStep : FindResultsPipelineStep<PagedQuery>
	{
		public override async Task ExecuteAsync(FindSettings find, PagedQuery query)
		{
			await Task.CompletedTask;

			find.Page = query.Page;
			find.PageSize = query.PageSize;
			find.MaxPages = query.MaxPages;
		}
	}
}