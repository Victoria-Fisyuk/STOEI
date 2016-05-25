﻿using System;
using System.Collections.Generic;
using System.Web.Mvc;
using TAiMStore.Domain;

namespace TAiMStore.Model.ViewModels
{
    public class OrderViewModel
    {
        [HiddenInput(DisplayValue = false)]
        public int Id { get; set; }

        public decimal TotalCost { get; set; }

        public string UserName { get; set; }

        public string eMail { get; set; }

        public string PaymentType { get; set; }
        
        public bool? Status { get; set; }

        [HiddenInput(DisplayValue = false)]
        public DateTime CreateDate { get; set; }

        public string ReturnUrl { get; set; }

        public void EntityToViewModel(Order order)
        {
            this.Id = order.Id;
            this.TotalCost = order.TotalCost;
            this.UserName = order.User.Name;
            this.eMail = order.User.Email;
            this.Status = order.Status;
            this.PaymentType = order.Payment.NameMethod;

            this.CreateDate = order.CreateDate;
        }
    }
}
