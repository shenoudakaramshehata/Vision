﻿using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace Vision.Models
{
    
    public class CustomerAddress
    {
        [Key]
        public int CustomerAddressId { get; set; }
        public string Adress { get; set; }
        public string Governorate { get; set; }
        public string Area { get; set; }
        public string Piece { get; set; }
        public string Avenue { get; set; }
        public string Street { get; set; }
        public string Building { get; set; }
        public string Floor { get; set; }
        public string ApartmentNumber { get; set; }
        public string Lat { get; set; }
        public string Lng { get; set; }
        public string UserId { get; set; }
    }
}