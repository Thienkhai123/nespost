using DevExpress.Web.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebSite.Models;

namespace WebSite.Controllers
{
    public class BaseController : Controller
    {

        // Paging
        public ActionResult PagingAction(GridViewPagerState pager, string partial)
        {
            var viewModel = GridViewExtension.GetViewModel("gridView");
            viewModel.ApplyPagingState(pager);
            return AdvancedCustomBindingCore(partial, viewModel);
        }
        // Filtering
        public ActionResult FilteringAction(GridViewFilteringState filteringState, string partial)
        {
            var viewModel = GridViewExtension.GetViewModel("gridView");
            viewModel.ApplyFilteringState(filteringState);
            return AdvancedCustomBindingCore(partial, viewModel);
        }
        // Sorting
        public ActionResult SortingAction(GridViewColumnState column, bool reset, string partial)
        {
            var viewModel = GridViewExtension.GetViewModel("gridView");
            viewModel.ApplySortingState(column, reset);
            return AdvancedCustomBindingCore(partial, viewModel);
        }
        // Grouping
        public ActionResult GroupingAction(GridViewColumnState column, string partial)
        {
            var viewModel = GridViewExtension.GetViewModel("gridView");
            viewModel.ApplyGroupingState(column);
            return AdvancedCustomBindingCore(partial, viewModel);
        }
        public PartialViewResult AdvancedCustomBindingCore(string partialView, GridViewModel viewModel)
        {
            viewModel.ProcessCustomBinding(
                GridViewCustomBindingHandlers.GetDataRowCountAdvanced,
                GridViewCustomBindingHandlers.GetDataAdvanced,
                GridViewCustomBindingHandlers.GetSummaryValuesAdvanced,
                GridViewCustomBindingHandlers.GetGroupingInfoAdvanced,
                GridViewCustomBindingHandlers.GetUniqueHeaderFilterValuesAdvanced
            );
            return PartialView(partialView, viewModel);
        }
    }
}