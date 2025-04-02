using System;
using System.ComponentModel.DataAnnotations;
using BouvetBackend.Entities;
using static System.Net.Mime.MediaTypeNames;

namespace BouvetBackend.Models.TransportEntryModel
{
    public class TransportEntryModel
    {
        public int UserId { get; set; }
        public required Methode Method { get; set; }  
        public double Co2 { get; set; }  
        public double DistanceKm { get; set; }  
        public double MoneySaved { get; set; }
        public required string StartingAddress { get; set; }     
    }
}



