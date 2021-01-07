using System.Threading.Tasks;
using System.Collections.Generic;
using Midnight.Pipeline;
using VirtualHole.DB.Contents;
using VirtualHole.DB.Creators;
using VirtualHole.API.Models;

namespace VirtualHole.API.Controllers
{
	public partial class ContentsController
	{
		public class ScheduledQueryStep : PipelineStep<FindContext<ContentQuery, Content>>
		{
			public override Task ExecuteAsync()
			{
				Context.InQuery.IsContentTypeInclude = true;
				Context.InQuery.ContentType = new List<string>() { ContentTypes.Video };

				Context.InQuery.IsCheckCreatorAffiliations = true;
				Context.InQuery.IsAffiliationsAll = false;
				Context.InQuery.IsAffiliationsInclude = false;

				if(Context.InQuery.CreatorAffiliations != null) {
					Context.InQuery.CreatorAffiliations.Add(AffiliationKeys.Community);
				} else {
					Context.InQuery.CreatorAffiliations = new List<string>() { AffiliationKeys.Community };
				}
				return Task.CompletedTask;
			}
		}
	}
}