using System;
using System.ComponentModel.DataAnnotations;
using static System.Net.Mime.MediaTypeNames;

namespace BouvetBackend.Models.TransportEntryModel
{

    public class CompositeViewModel
    {
        public TransportEntryModel ?TransportEntryModel { get; set; }
            }
    public class TransportEntryFullModel
    {
        public TransportEntryFullModel()
        {
            UpsertModel = new TransportEntryModel();
            TransportEntryModelList = new List<TransportEntryModel>();
        }

        public TransportEntryModel UpsertModel { get; set; }
        public List<TransportEntryModel> TransportEntryModelList { get; set; }
    }

    public class TransportEntryModel
    {
        public int UserId { get; set; }
        public string ?Email { get; set; } 
        public string ?Method { get; set; }  
        public double Co2 { get; set; }  
        public double DistanceKm { get; set; }  
        public string? StartingAddress { get; set; }     
    }
}



