using Microsoft.AspNetCore.Mvc;
using Surma.Cms.Core.DynamicPages;

namespace Surma.Cms.Core.Pages;

public class CmsPage : CmsRazorPageBase
{
    public override async Task<IActionResult> OnGetAsync()
    {
        // Check if cms page is accessed directly
        if (Request.Path.Value?.ToLower() == "/cmspage")
        {
            return NotFound();
        }
        
        return await base.OnGetAsync();
    }
}