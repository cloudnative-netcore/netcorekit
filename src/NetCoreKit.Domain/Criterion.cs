using System;

namespace NetCoreKit.Domain
{
  public class Criterion
  {
    private const int MaxPageSize = 50;
    private const int ConfigurablePageSize = 10;

    public Criterion()
    {
      CurrentPage = 0;
      PageSize = ConfigurablePageSize;
      SortBy = "Id";
      SortOrder = "desc";
    }

    public int CurrentPage { get; set; }

    private int _pageSize = MaxPageSize;
    public int PageSize
    {
      get => _pageSize;
	    set => _pageSize = (value > MaxPageSize) ? MaxPageSize :
		    (value < ConfigurablePageSize ? ConfigurablePageSize : value);
    }

    public string SortBy { get; private set; }
    public string SortOrder { get; private set; }

    public Criterion SetPageSize(int pageSize)
    {
      if (pageSize <= 0)
        throw new Exception("PageSize could not be less than zero.");

      PageSize = pageSize;
      return this;
    }

    public Criterion SetCurrentPage(int currentPage)
    {
      if (currentPage <= 0)
        throw new Exception("CurrentPage could not be less than zero.");

      CurrentPage = currentPage;
      return this;
    }
  }
}
