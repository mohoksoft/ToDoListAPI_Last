using Newtonsoft.Json;
using System.Linq;
using ToDoListAPI.Models;

namespace ToDoListAPI
{

    // A class that handles pagination of an IQueryable

    public class CNPagedList<T>
    {
        /*
        * list - The full list of items you would like to paginate</param>
        * page - (optional) The current page number</param>
        * pageSize - (optional) The size of the page</param>
        */
        public CNPagedList(IQueryable<T> list, int? page = null, int? pageSize = null)
        {
            _list = list;
            _page = page;
            _pageSize = pageSize;
        }

        private IQueryable<T> _list;

        // The paginated result

        public string items
        {
            get
            {
                if (_list == null) return null;

                var json = JsonConvert.SerializeObject(_list.Skip((page - 1) * pageSize).Take(pageSize).ToArray());

                return json;
            }
        }

        private int? _page;

        //  The current page.

        public int page
        {
            get
            {
                if (!_page.HasValue)
                {
                    return 1;
                }
                else
                {
                    return _page.Value;
                }
            }
        }

        private int? _pageSize;

        // The size of the page.

        public int pageSize
        {
            get
            {
                if (!_pageSize.HasValue)
                {
                    return _list == null ? 0 : _list.Count();
                }
                else
                {
                    return _pageSize.Value;
                }
            }
        }


        // The total number of items in the original list of items.

        public int totalItemCount
        {
            get
            {
                return _list == null ? 0 : _list.Count();
            }
        }
    }

}