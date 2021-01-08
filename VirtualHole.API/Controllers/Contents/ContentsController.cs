﻿using System.Web.Http;
using System.Threading.Tasks;
using Midnight.Pipeline;
using VirtualHole.DB;
using VirtualHole.DB.Contents;
using VirtualHole.DB.Creators;
using VirtualHole.API.Models;
using VirtualHole.API.Services;

namespace VirtualHole.API.Controllers
{
	public partial class ContentsController : ApiController
	{
		private CreatorsClient creatorsClient => dbService.Client.Creators;
		private ContentsClient contentsClient => dbService.Client.Contents;
		private VirtualHoleDBService dbService = null;

		public ContentsController()
		{
			dbService = new VirtualHoleDBService();
		}

		[Route("api/v1/contents/discover")]
		[HttpGet]
		public async Task<IHttpActionResult> GetDiscover([FromUri] ContentQuery query)
		{
			Pipeline<FindContext<ContentQuery, Content>> pipeline =
				new Pipeline<FindContext<ContentQuery, Content>>(
					new FindContext<ContentQuery, Content>() {
						InQuery = query ?? new ContentQuery(),
						InProvider = (FindSettings find) => contentsClient.FindAsync(find),
						InPostProcess = ContentDTOPostProcess.ContentDTOFactory
					});

			pipeline.Add(new DiscoverQueryStep());
			pipeline.Add(new ContentFilterStep(creatorsClient));
			pipeline.Add(new ContentSortStep());
			pipeline.Add(new GetPagedResultsStep<ContentQuery, Content>());
			await pipeline.ExecuteAsync();

			return Ok(pipeline.Context.OutResults);
		}

		[Route("api/v1/contents/community")]
		[HttpGet]
		public async Task<IHttpActionResult> GetCommunity([FromUri] ContentQuery query)
		{
			Pipeline<FindContext<ContentQuery, Content>> pipeline =
				new Pipeline<FindContext<ContentQuery, Content>>(
					new FindContext<ContentQuery, Content>() {
						InQuery = query ?? new ContentQuery(),
						InProvider = (FindSettings find) => contentsClient.FindAsync(find),
						InPostProcess = ContentDTOPostProcess.ContentDTOFactory
					});

			pipeline.Add(new CommunityQueryStep());
			pipeline.Add(new ContentFilterStep(creatorsClient));
			pipeline.Add(new ContentSortStep());
			pipeline.Add(new GetPagedResultsStep<ContentQuery, Content>());
			await pipeline.ExecuteAsync();

			return Ok(pipeline.Context.OutResults);
		}

		[Route("api/v1/contents/live")]
		[HttpGet]
		public async Task<IHttpActionResult> GetLive([FromUri] ContentQuery query)
		{
			Pipeline<FindContext<ContentQuery, Content>> pipeline =
				new Pipeline<FindContext<ContentQuery, Content>>(
					new FindContext<ContentQuery, Content>() {
						InQuery = query ?? new ContentQuery(),
						InProvider = (FindSettings find) => contentsClient.FindAsync(find),
						InPostProcess = ContentDTOPostProcess.ContentDTOFactory
					});

			pipeline.Add(new LiveQueryStep());
			pipeline.Add(new ContentFilterStep(creatorsClient));
			pipeline.Add(new BroadcastLiveFilterStep() { IsLive = true });
			pipeline.Add(new ContentSortStep());
			pipeline.Add(new GetPagedResultsStep<ContentQuery, Content>());
			await pipeline.ExecuteAsync();

			return Ok(pipeline.Context.OutResults);
		}

		[Route("api/v1/contents/scheduled")]
		[HttpGet]
		public async Task<IHttpActionResult> GetScheduled([FromUri] ContentQuery query)
		{
			Pipeline<FindContext<ContentQuery, Content>> pipeline =
				new Pipeline<FindContext<ContentQuery, Content>>(
					new FindContext<ContentQuery, Content>() {
						InQuery = query ?? new ContentQuery(),
						InProvider = (FindSettings find) => contentsClient.FindAsync(find),
						InPostProcess = ContentDTOPostProcess.ContentDTOFactory
					});

			pipeline.Add(new ScheduledQueryStep());
			pipeline.Add(new ContentFilterStep(creatorsClient));
			pipeline.Add(new BroadcastLiveFilterStep() { IsLive = false });
			pipeline.Add(new BroadcastSortStep());
			pipeline.Add(new GetPagedResultsStep<ContentQuery, Content>());
			await pipeline.ExecuteAsync();

			return Ok(pipeline.Context.OutResults);
		}
	}
}