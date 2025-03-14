//EXAMPLE refrence

using System;
using System.ComponentModel.DataAnnotations;
using static System.Net.Mime.MediaTypeNames;

namespace BouvetBackend.Models.ApiModel
{

    public class CompositeViewModel
    {
        public ApiModel ?APIModel { get; set; }
            }
    public class ApiFullModel
    {
        public ApiFullModel()
        {
            UpsertModel = new ApiModel();
            ApiModelList = new List<ApiModel>();
        }

        public ApiModel UpsertModel { get; set; }
        public List<ApiModel> ApiModelList { get; set; }
    }

    public class ApiModel
    {
        [Required]
        public int apiId { get; set; }
        public int value1 { get; set; }
        public int value2 { get; set; }
    }
}

