using WebApiAutofacCrud.Models;

namespace WebApiAutofacCrud.Dtos.Responses.Shared
{
    public abstract class PagedDto : SuccessResponse
    {
        public PageMeta PageMeta { get; set; }
    }
}