using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Web;
using System.Web.Mvc.Ajax;
using System.Web.Mvc;
using System.Web.Routing;
using System.Text;

namespace MvcPaging
{
    public static class AjaxPagingExtensions
    {
        //
        // Summary:
        //     Create Pager with different type of options like custom page title, tooltip,
        //     font size, controls option.
        //     new Options {
        //         PageSize = Model.PageSize,
        //         TotalItemCount = Model.TotalItemCount,
        //         CurrentPage = Model.PageNumber,
        //         ItemTexts = new ItemTexts() { Next = "Next", Previous = "Previous", Page
        //     = "P" },
        //         TooltipTitles = new TooltipTitles() { Next = "Next page", Previous = "Previous
        //     page", Page = "Page" },
        //         Size = Size.normal,
        //         Alignment = Alignment.centered,
        //         IsShowControls = true },
        //     new AjaxOptions {
        //         UpdateTargetId = "grid-list",
        //         OnBegin = "beginPaging",
        //         OnSuccess = "successPaging",
        //         OnFailure = "failurePaging" },
        //     new { filterParameter = ViewData["foo"] })
        //
        // Parameters:
        //   htmlHelper:
        //
        //   options:
        //
        //   ajaxOptions:
        //
        //   values:
        //     Set your fileter parameter
        //     new { parameterName = ViewData["foo"] }
        public static string Pager(this AjaxHelper ajaxHelper, Options options, AjaxOptions ajaxOptions, object values)
        {
            return ajaxHelper.Pager(options, ajaxOptions, new RouteValueDictionary(values));
        }

        public static string Pager(this AjaxHelper ajaxHelper, Options options, AjaxOptions ajaxOptions, RouteValueDictionary valuesDictionary)
        {
            if (valuesDictionary == null)
            {
                valuesDictionary = new RouteValueDictionary();
            }

            if (options.ActionName != null)
            {
                if (valuesDictionary.ContainsKey("action"))
                {
                    throw new ArgumentException("The valuesDictionary already contains an action.", "actionName");
                }

                valuesDictionary.Add("action", options.ActionName);
            }

            AjaxPager ajaxPager = new AjaxPager(ajaxHelper, ajaxHelper.ViewContext, options, ajaxOptions, valuesDictionary);
            return ajaxPager.RenderHtml();
        }
    }
    public static class IEnumerableExtensions
    {
        public static IPagedList<T> ToPagedList<T>(this IEnumerable<T> source, int pageIndex, int pageSize)
        {
            return new PagedList<T>(source, pageIndex, pageSize);
        }

        public static IPagedList<T> ToPagedList<T>(this IEnumerable<T> source, int pageIndex, int pageSize, int totalCount)
        {
            return new PagedList<T>(source, pageIndex, pageSize, totalCount);
        }
    }
    public class AjaxPager
    {
        private AjaxHelper ajaxHelper;

        private ViewContext viewContext;

        private readonly Options options;

        private readonly RouteValueDictionary linkWithoutPageValuesDictionary;

        private readonly AjaxOptions ajaxOptions;

        public AjaxPager(AjaxHelper helper, ViewContext viewContext, Options options, AjaxOptions ajaxOptions, RouteValueDictionary valueDictionary)
        {
            ajaxHelper = helper;
            this.viewContext = viewContext;
            this.options = options;
            this.ajaxOptions = ajaxOptions;
            linkWithoutPageValuesDictionary = valueDictionary;
        }

        public string RenderHtml()
        {
            int num = (int)Math.Ceiling((double)options.TotalItemCount / (double)options.PageSize);
            int num2 = 10;
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.AppendFormat("<div class=\"pagination pagination-{0} pagination-{1} {2}\"><ul>", options.Size.ToString(), options.Alignment.ToString(), options.CssClass);
            if (options.IsShowFirstLast)
            {
                if (options.CurrentPage > 1)
                {
                    stringBuilder.AppendFormat("<li class=\"first\">{0}</li>", GeneratePageLink(options.ItemTexts.First, 1, options.TooltipTitles.First, options.ItemIcon.First));
                }
                else if (string.IsNullOrWhiteSpace(options.ItemIcon.First))
                {
                    stringBuilder.AppendFormat("<li class=\"active first\"><span>{0}</span></li>", options.ItemTexts.First);
                }
                else
                {
                    stringBuilder.AppendFormat("<li class=\"active first\"><span><i class=\"{1}\"></i> {0}</span></li>", options.ItemTexts.First, options.ItemIcon.First);
                }
            }

            if (options.IsShowControls)
            {
                if (options.CurrentPage > 1)
                {
                    stringBuilder.AppendFormat("<li class=\"previous\">{0}</li>", GeneratePageLink(options.ItemTexts.Previous, options.CurrentPage - 1, options.TooltipTitles.Previous, options.ItemIcon.Previous));
                }
                else if (string.IsNullOrWhiteSpace(options.ItemIcon.Previous))
                {
                    stringBuilder.AppendFormat("<li class=\"active previous\"><span>{0}</span></li>", options.ItemTexts.Previous);
                }
                else
                {
                    stringBuilder.AppendFormat("<li class=\"active previous\"><span><i class=\"{1}\"></i> {0}</span></li>", options.ItemTexts.Previous, options.ItemIcon.Previous);
                }
            }

            int num3 = 1;
            int num4 = num;
            if (options.IsShowPages)
            {
                if (num > num2)
                {
                    int num5 = (int)Math.Ceiling((double)num2 / 2.0) - 1;
                    int num6 = options.CurrentPage - num5;
                    int num7 = options.CurrentPage + num5;
                    if (num6 < 4)
                    {
                        num7 = num2;
                        num6 = 1;
                    }
                    else if (num7 > num - 4)
                    {
                        num7 = num;
                        num6 = num - num2;
                    }

                    num3 = num6;
                    num4 = num7;
                }

                if (!options.IsShowFirstLast && num3 > 3)
                {
                    stringBuilder.AppendFormat("<li>{0}</li>", GeneratePageLink(string.Format("{0}{1}", options.ItemTexts.Page, "1"), 1, options.TooltipTitles.Page, options.ItemIcon.Page));
                    stringBuilder.AppendFormat("<li>{0}</li>", GeneratePageLink(string.Format("{0}{1}", options.ItemTexts.Page, "2"), 2, options.TooltipTitles.Page, options.ItemIcon.Page));
                    stringBuilder.Append("<li class=\"disabled\"><span>...</span></li>");
                }

                for (int i = num3; i <= num4; i++)
                {
                    if (i == options.CurrentPage)
                    {
                        stringBuilder.AppendFormat("<li class=\"active\"><span>{0}</span></li>", $"{options.ItemTexts.Page}{i}");
                    }
                    else
                    {
                        stringBuilder.AppendFormat("<li>{0}</li>", GeneratePageLink($"{options.ItemTexts.Page}{i.ToString()}", i, options.TooltipTitles.Page, options.ItemIcon.Page));
                    }
                }

                if (!options.IsShowFirstLast && num4 < num - 3)
                {
                    stringBuilder.Append("<li class=\"disabled\"><span>...</span></li>");
                    stringBuilder.AppendFormat("<li>{0}</li>", GeneratePageLink($"{options.ItemTexts.Page}{(num - 1).ToString()}", num - 1, options.TooltipTitles.Page, options.ItemIcon.Page));
                    stringBuilder.AppendFormat("<li>{0}</li>", GeneratePageLink($"{options.ItemTexts.Page}{num.ToString()}", num, options.TooltipTitles.Page, options.ItemIcon.Page));
                }
            }

            if (options.IsShowControls)
            {
                if (options.CurrentPage < num)
                {
                    stringBuilder.AppendFormat("<li class=\"next\">{0}</li>", GeneratePageLink(options.ItemTexts.Next, options.CurrentPage + 1, options.TooltipTitles.Next, options.ItemIcon.Next, "last"));
                }
                else if (string.IsNullOrWhiteSpace(options.ItemIcon.Next))
                {
                    stringBuilder.AppendFormat("<li class=\"active next\"><span>{0}</span></li>", options.ItemTexts.Next);
                }
                else
                {
                    stringBuilder.AppendFormat("<li class=\"active next\"><span>{0} <i class=\"{1}\"></i></span></li>", options.ItemTexts.Next, options.ItemIcon.Next);
                }
            }

            if (options.IsShowFirstLast)
            {
                if (options.CurrentPage < num)
                {
                    stringBuilder.AppendFormat("<li class=\"last\">{0}</li>", GeneratePageLink(options.ItemTexts.Last, num, options.TooltipTitles.Last, options.ItemIcon.Last, "last"));
                }
                else if (string.IsNullOrWhiteSpace(options.ItemIcon.Last))
                {
                    stringBuilder.AppendFormat("<li class=\"active last\"><span>{0}</span></li>", options.ItemTexts.Last);
                }
                else
                {
                    stringBuilder.AppendFormat("<li class=\"active last\"><span>{0} <i class=\"{1}\"></i></span></li>", options.ItemTexts.Last, options.ItemIcon.Last);
                }
            }

            stringBuilder.Append("</ul></div>");
            return stringBuilder.ToString();
        }

        private MvcHtmlString GeneratePageLink(string linkText, int pageNumber, string title, string icon, string buttonType = "")
        {
            MvcHtmlString mvcHtmlString = new MvcHtmlString(string.Empty);
            RouteValueDictionary routeValueDictionary = new RouteValueDictionary(linkWithoutPageValuesDictionary);
            routeValueDictionary.Add("page", pageNumber);
            string value = string.Format(title, pageNumber);
            RouteValueDictionary routeValueDictionary2 = new RouteValueDictionary();
            routeValueDictionary2.Add("title", value);
            if (string.IsNullOrWhiteSpace(icon))
            {
                return ajaxHelper.ActionLink(linkText, routeValueDictionary["action"].ToString(), routeValueDictionary, ajaxOptions, routeValueDictionary2);
            }

            string empty = string.Empty;
            TagBuilder tagBuilder = new TagBuilder("i");
            tagBuilder.MergeAttribute("class", icon);
            empty = ((!string.IsNullOrWhiteSpace(buttonType)) ? ajaxHelper.ActionLink(linkText + " [replaceIcon]", routeValueDictionary["action"].ToString(), routeValueDictionary, ajaxOptions, routeValueDictionary2).ToHtmlString() : ajaxHelper.ActionLink("[replaceIcon] " + linkText, routeValueDictionary["action"].ToString(), routeValueDictionary, ajaxOptions, routeValueDictionary2).ToHtmlString());
            return new MvcHtmlString(empty.Replace("[replaceIcon]", tagBuilder.ToString()));
        }
    }
    public class ItemTexts
    {
        private string next = "»";

        private string previous = "«";

        private string page = "";

        private string first = "First";

        private string last = "Last";

        //
        // Summary:
        //     Default value "»"
        public string Next
        {
            get
            {
                return next;
            }
            set
            {
                next = value;
            }
        }

        //
        // Summary:
        //     Default value "«"
        public string Previous
        {
            get
            {
                return previous;
            }
            set
            {
                previous = value;
            }
        }

        //
        // Summary:
        //     Default value null
        public string Page
        {
            get
            {
                return page;
            }
            set
            {
                page = value;
            }
        }

        //
        // Summary:
        //     Default value First
        public string First
        {
            get
            {
                return first;
            }
            set
            {
                first = value;
            }
        }

        //
        // Summary:
        //     Default value Last
        public string Last
        {
            get
            {
                return last;
            }
            set
            {
                last = value;
            }
        }
    }
    public class ItemIcon
    {
        private string next = string.Empty;

        private string previous = string.Empty;

        private string page = string.Empty;

        private string first = string.Empty;

        private string last = string.Empty;

        //
        // Summary:
        //     Default value string.Empty
        public string Next
        {
            get
            {
                return next;
            }
            set
            {
                next = value;
            }
        }

        //
        // Summary:
        //     Default value string.Empty
        public string Previous
        {
            get
            {
                return previous;
            }
            set
            {
                previous = value;
            }
        }

        //
        // Summary:
        //     Default value string.Empty
        public string Page
        {
            get
            {
                return page;
            }
            set
            {
                page = value;
            }
        }

        //
        // Summary:
        //     Default value string.Empty
        public string First
        {
            get
            {
                return first;
            }
            set
            {
                first = value;
            }
        }

        //
        // Summary:
        //     Default value string.Empty
        public string Last
        {
            get
            {
                return last;
            }
            set
            {
                last = value;
            }
        }
    }
    public class TooltipTitles
    {
        private string next = "Go to next page";

        private string previous = "Go to previous page";

        private string page = "";

        private string first = "Go to first page";

        private string last = "Go to last page";

        //
        // Summary:
        //     Default value "Go to next page"
        public string Next
        {
            get
            {
                return next;
            }
            set
            {
                next = value;
            }
        }

        //
        // Summary:
        //     Default value "Go to previous page"
        public string Previous
        {
            get
            {
                return previous;
            }
            set
            {
                previous = value;
            }
        }

        public string Page
        {
            get
            {
                return page;
            }
            set
            {
                page = value;
            }
        }

        //
        // Summary:
        //     Default value "Go to first page"
        public string First
        {
            get
            {
                return first;
            }
            set
            {
                first = value;
            }
        }

        //
        // Summary:
        //     Default value "Go to last page"
        public string Last
        {
            get
            {
                return last;
            }
            set
            {
                last = value;
            }
        }
    }
    public enum Alignment
    {
        left,
        centered,
        right
    }
    public enum Size
    {
        normal,
        mini,
        small,
        large
    }
    public class Options
    {
        private ItemTexts _itemTexts;

        private ItemIcon _itemIcon;

        private TooltipTitles _tooltipTitles;

        private bool _isShowControls = true;

        private bool _isShowFirstLast = false;

        private bool _isShowPages = true;

        //
        // Summary:
        //     Set curent page value
        public int CurrentPage;

        //
        // Summary:
        //     Set page size
        public int PageSize;

        //
        // Summary:
        //     Set total item count
        public int TotalItemCount;

        //
        // Summary:
        //     Set action name
        public string ActionName;

        //
        // Summary:
        //     Set font size normal, mini, small, large
        //     Size = Size.normal
        public Size Size { get; set; }

        //
        // Summary:
        //     Set Alignment left, centered right
        //     Alignment = Alignment.centered
        public Alignment Alignment { get; set; }

        //
        // Summary:
        //     Set Paging Next, Previous, Page value
        //     ItemTexts = new ItemTexts() { Next = "»", Previous = "«", Page = "" }
        public ItemTexts ItemTexts
        {
            get
            {
                return (_itemTexts == null) ? new ItemTexts() : _itemTexts;
            }
            set
            {
                _itemTexts = value;
            }
        }

        //
        // Summary:
        //     Set Paging Next, Previous, Page icon class
        //     ItemIcon = new ItemIcon() { Next = "icon-chevron-right", Previous = "icon-chevron-left"
        //     }
        public ItemIcon ItemIcon
        {
            get
            {
                return (_itemIcon == null) ? new ItemIcon() : _itemIcon;
            }
            set
            {
                _itemIcon = value;
            }
        }

        //
        // Summary:
        //     Set title tooltip for next, previous and page link
        //     TooltipTitles = new TooltipTitles() { Next = "Next page", Previous = "Previous
        //     page", Page = "Page" }
        public TooltipTitles TooltipTitles
        {
            get
            {
                return (_tooltipTitles == null) ? new TooltipTitles() : _tooltipTitles;
            }
            set
            {
                _tooltipTitles = value;
            }
        }

        //
        // Summary:
        //     Set bool value for next and previous button
        public bool IsShowControls
        {
            get
            {
                return _isShowControls;
            }
            set
            {
                _isShowControls = value;
            }
        }

        //
        // Summary:
        //     Set bool value for first and last button
        public bool IsShowFirstLast
        {
            get
            {
                return _isShowFirstLast;
            }
            set
            {
                _isShowFirstLast = value;
            }
        }

        //
        // Summary:
        //     Set bool value for 1,2,3,4,5 Paging list, Default value is true
        public bool IsShowPages
        {
            get
            {
                return _isShowPages;
            }
            set
            {
                _isShowPages = value;
            }
        }

        //
        // Summary:
        //     Set css class for custom design
        public string CssClass { get; set; }
    }
}