﻿using MainMusicStore.DataAccess.IMainRepository;
using MainMusicStore.Models.DbModels;
using MainMusicStore.Models.ViewModels;
using MainMusicStore.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Stripe;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;

namespace MainMusicStore.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize]
    public class OrderController : Controller
    {
        private readonly IUnitOfWork _uow;
        public OrderController(IUnitOfWork uow)
        {
            _uow = uow;
        }
       
        [BindProperty]
        public OrderDetailsVM OrderDetailVM { get; set; }

        public IActionResult Index()
        {
            return View();
        }

       public IActionResult Details(int id)
        {
            OrderDetailVM = new OrderDetailsVM
            {
                OrderHeader = _uow.OrderHeader.GetFirstOrDefault(u => u.Id == id, includeProperties: "ApplicationUser"),
                OrderDetails = _uow.OrderDetail.GetAll(o => o.OrderId == id, includeProperties: "Product")
            };

            return View(OrderDetailVM);
        }

        [Authorize(Roles = ProjectConstant.Role_Admin + "," + ProjectConstant.Role_Employee)]
        public IActionResult StartProcessing(int id)
        {
            OrderHeader orderHeader = _uow.OrderHeader.GetFirstOrDefault(u => u.Id == id);
            orderHeader.OrderStatus = ProjectConstant.StatusInProcess;
            _uow.Save();
            return RedirectToAction("Index");
        }

        [HttpPost]
        [Authorize(Roles = ProjectConstant.Role_Admin + "," + ProjectConstant.Role_Employee)]
        public IActionResult ShipOrder()
        {
            OrderHeader orderHeader = _uow.OrderHeader.GetFirstOrDefault(u => u.Id == OrderDetailVM.OrderHeader.Id);
            orderHeader.TrackingNumber = OrderDetailVM.OrderHeader.TrackingNumber;
            orderHeader.Carrier = OrderDetailVM.OrderHeader.Carrier;
            orderHeader.OrderStatus = ProjectConstant.StatusShipped;
            orderHeader.ShippingDate = DateTime.Now;

            _uow.Save();
            return RedirectToAction("Index");
        }


        [Authorize(Roles = ProjectConstant.Role_Admin + "," + ProjectConstant.Role_Employee)]
        public IActionResult CancelOrder(int id)
        {
            OrderHeader orderHeader = _uow.OrderHeader.GetFirstOrDefault(u => u.Id == id);

            if (orderHeader.PaymentStatus == ProjectConstant.StatusApproved)
            {
                var options = new RefundCreateOptions
                {
                    Amount = Convert.ToInt32(orderHeader.OrderTotal * 100),
                    Reason = RefundReasons.RequestedByCustomer,
                    Charge = orderHeader.TransactionId
                };

                var service = new RefundService();
               // Refund refund = service.Create(options);

                orderHeader.OrderStatus = ProjectConstant.StatusRefund;
                orderHeader.PaymentStatus = ProjectConstant.StatusRefund;
            }
            else
            {
                orderHeader.OrderStatus = ProjectConstant.StatusCancelled;
                orderHeader.PaymentStatus = ProjectConstant.StatusCancelled;
            }
            _uow.Save();
            return RedirectToAction("Index");
        }

        public IActionResult UpdateOrderDetails()
        {
            var orderHEaderFromDb = _uow.OrderHeader.GetFirstOrDefault(u => u.Id == OrderDetailVM.OrderHeader.Id);
            orderHEaderFromDb.Name = OrderDetailVM.OrderHeader.Name;
            orderHEaderFromDb.PhoneNumber = OrderDetailVM.OrderHeader.PhoneNumber;
            orderHEaderFromDb.StreetAddress = OrderDetailVM.OrderHeader.StreetAddress;
            orderHEaderFromDb.City = OrderDetailVM.OrderHeader.City;
            orderHEaderFromDb.State = OrderDetailVM.OrderHeader.State;
            orderHEaderFromDb.PostCode = OrderDetailVM.OrderHeader.PostCode;
            if (OrderDetailVM.OrderHeader.Carrier != null)
            {
                orderHEaderFromDb.Carrier = OrderDetailVM.OrderHeader.Carrier;
            }
            if (OrderDetailVM.OrderHeader.TrackingNumber != null)
            {
                orderHEaderFromDb.TrackingNumber = OrderDetailVM.OrderHeader.TrackingNumber;
            }

            _uow.Save();
            TempData["Error"] = "Order Details Updated Successfully.";
            return RedirectToAction("Details", "Order", new { id = orderHEaderFromDb.Id });
        }

        #region APICALLS

        [HttpGet]
        public IActionResult GetOrderList(string status)
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);

            IEnumerable<OrderHeader> orderHeaderList;

            if (User.IsInRole(ProjectConstant.Role_Admin) || User.IsInRole(ProjectConstant.Role_Employee))
                orderHeaderList = _uow.OrderHeader.GetAll(includeProperties: "ApplicationUser");
            else
                orderHeaderList = _uow.OrderHeader.GetAll(u => u.ApplicationUserId == claim.Value, includeProperties: "ApplicationUser");
           
            switch (status)
            {
                case "pending":
                    orderHeaderList = orderHeaderList.Where(o => o.PaymentStatus == ProjectConstant.PaymentStatusDelayedPayment);
                    break;

                case "inprocess":
                    orderHeaderList = orderHeaderList.Where(o =>
                                        o.OrderStatus == ProjectConstant.StatusApproved
                                        || o.OrderStatus == ProjectConstant.StatusInProcess
                                        || o.OrderStatus == ProjectConstant.StatusPending);
                    break;

                case "completed":
                    orderHeaderList = orderHeaderList.Where(o => o.OrderStatus == ProjectConstant.StatusShipped);
                    break;

                case "rejected":
                    orderHeaderList = orderHeaderList.Where(o => o.OrderStatus == ProjectConstant.StatusCancelled
                                                            || o.OrderStatus == ProjectConstant.StatusRefund
                                                            || o.OrderStatus == ProjectConstant.PaymentStatusRejected);
                    break;

                default:
                    break;
            }


            //  orderHeaderList = _uow.OrderHeader.GetAll(includeProperties: "ApplicationUser");

            return Json(new { data = orderHeaderList });
        }

        #endregion

    }
}