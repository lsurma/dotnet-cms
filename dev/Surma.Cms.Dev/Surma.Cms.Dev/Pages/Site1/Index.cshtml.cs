using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Surma.Cms.Core;
using Surma.CMS.Dev.CMS;
using Surma.CMS.Dev.Scrutor;

namespace Surma.CMS.Dev.Pages;

public class IndexModel : PageModel
{
    private readonly ILogger<IndexModel> _logger;

    public IndexModel(
        ILogger<IndexModel> logger
    )
    {
        _logger = logger;
    }

    public async Task OnGetAsync()
    {
        
    }
}