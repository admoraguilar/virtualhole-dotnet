using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using VirtualHole.DB;
using VirtualHole.API.Models;

namespace VirtualHole.API.Controllers
{
	public class FindContext<TQuery, TResult>
		where TQuery : PagedQuery
	{
		public Func<FindSettings, Task<FindResults<TResult>>> InProvider { get; set; } = null;
		public Func<TQuery, TResult, Task<object>> InPostProcess { get; set; } = null;

		public TQuery InQuery { get; set; } = null;
		public FindSettings InFindSettings { get; set; } = new FindSettings();

		public List<object> OutResults { get; set; } = new List<object>();
	}
}