using System.Net.NetworkInformation;
using ArfFipaso.Pagination.Models;

namespace Common.Definitions.Base.Logics;

public class PaginationLogics
{
	public static (XPageResponse PageResponse, IQueryable<T> Data) GetPageInfo<T>(XPageRequest pageRequest, IQueryable<T> queryableData)
	{
		if (pageRequest == null)
			pageRequest = new XPageRequest()
			{
				PerPageCount = 20,
				CurrentPage = 1,
				ListAll = false,
			};

		var totalRowCount = queryableData?.ToList().Count ?? 0;
		var totalPageCount = totalRowCount / pageRequest.PerPageCount;
		if ((totalPageCount * pageRequest.PerPageCount) < totalRowCount)
			totalPageCount++;

		var pageResponse = new XPageResponse()
		{
			PerPageCount = pageRequest.PerPageCount,
			CurrentPage = pageRequest.ListAll ? -1 : pageRequest.CurrentPage,
			TotalRowCount = totalRowCount,
			TotalPageCount = totalPageCount,
			HasNextPage = pageRequest.ListAll ? false : pageRequest.CurrentPage < totalPageCount,
			HasPreviousPage = pageRequest.ListAll ? false : pageRequest.CurrentPage > 1,
			ListAll = pageRequest.ListAll,
		};

		IQueryable<T> slice;
		if (!pageRequest.ListAll)
		{
			var skipCount = pageRequest.PerPageCount * (pageRequest.CurrentPage - 1);
			slice = queryableData.Skip(skipCount).Take(pageRequest.PerPageCount);
		}
		else
		{
			slice = queryableData.AsQueryable();
		}

		return (pageResponse, slice);
	}
}
