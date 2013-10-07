using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using System.Collections.ObjectModel;
using jQuery.DataTables.Mvc;
using SL8VendorPortal.Infrastructure;


namespace SL8VendorPortal.Models
{

    public static class InMemoryTransferOrdersRepository
    {
        public static IList<transfer> AllTransferOrders { get; set; }

        public static IList<transfer> GetTransferOrders(int startIndex, int pageSize, ReadOnlyCollection<SortedColumn> sortedColumns, out int totalRecordCount,
            out int searchRecordCount, string searchString)
        {
            var transfers = AllTransferOrders;

            totalRecordCount = transfers.Count;

            if (!string.IsNullOrWhiteSpace(searchString))
            {
                transfers = transfers.Where(c => c.trn_num.ToLower().Contains(searchString.ToLower()))
                    .ToList();
            }

            searchRecordCount = transfers.Count;

            IOrderedEnumerable<transfer> sortedList = null;
            foreach (var sortedColumn in sortedColumns)
            {
                switch (sortedColumn.PropertyName)
                {
                    case "trn_num":
                        sortedList = sortedList == null ? transfers.CustomSort(sortedColumn.Direction, i => i.trn_num)
                            : sortedList.CustomSort(sortedColumn.Direction, i => i.trn_num);
                        break;
                    case "from_whse":
                        sortedList = sortedList == null ? transfers.CustomSort(sortedColumn.Direction, i => i.from_whse)
                            : sortedList.CustomSort(sortedColumn.Direction, i => i.from_whse);
                        break;
                    case "to_whse":
                        sortedList = sortedList == null ? transfers.CustomSort(sortedColumn.Direction, i => i.to_whse)
                            : sortedList.CustomSort(sortedColumn.Direction, i => i.to_whse);
                        break;
                    case "stat":
                        sortedList = sortedList == null ? transfers.CustomSort(sortedColumn.Direction, i => i.stat)
                            : sortedList.CustomSort(sortedColumn.Direction, i => i.stat);
                        break;
                    case "ship_code":
                        sortedList = sortedList == null ? transfers.CustomSort(sortedColumn.Direction, i => i.ship_code)
                            : sortedList.CustomSort(sortedColumn.Direction, i => i.ship_code);
                        break;
                    case "weight":
                        sortedList = sortedList == null ? transfers.CustomSort(sortedColumn.Direction, i => i.weight)
                            : sortedList.CustomSort(sortedColumn.Direction, i => i.weight);
                        break;
                    case "qty_packages":
                        sortedList = sortedList == null ? transfers.CustomSort(sortedColumn.Direction, i => i.qty_packages)
                            : sortedList.CustomSort(sortedColumn.Direction, i => i.qty_packages);
                        break;
                    case "CreatedBy":
                        sortedList = sortedList == null ? transfers.CustomSort(sortedColumn.Direction, i => i.CreatedBy)
                            : sortedList.CustomSort(sortedColumn.Direction, i => i.CreatedBy);
                        break;
                    case "UpdatedBy":
                        sortedList = sortedList == null ? transfers.CustomSort(sortedColumn.Direction, i => i.UpdatedBy)
                            : sortedList.CustomSort(sortedColumn.Direction, i => i.UpdatedBy);
                        break;
                    case "CreateDate":
                        sortedList = sortedList == null ? transfers.CustomSort(sortedColumn.Direction, i => i.CreateDate)
                            : sortedList.CustomSort(sortedColumn.Direction, i => i.CreateDate);
                        break;
                    case "order_date":
                        sortedList = sortedList == null ? transfers.CustomSort(sortedColumn.Direction, i => i.order_date)
                            : sortedList.CustomSort(sortedColumn.Direction, i => i.order_date);
                        break;
                }
            }
            /*I had a problem in my Datatable when adding the drill down functionality, because when I added a null column to hold my drill down image, this passed a null value to 'Sort' which caused an exception here.
             * I fixed that by adding the try catch loop below.
             */
            try { return sortedList.Skip(startIndex).Take(pageSize).ToList(); }
            catch (ArgumentNullException)
            { return transfers.Skip(startIndex).Take(pageSize).ToList(); }
        }
    }

    public static class InMemoryTransferOrderLinesRepository
    {
        public static IList<trnitem> AllTransferOrderLines { get; set; }

        public static IList<trnitem> GetTransferOrderLines(int startIndex, int pageSize, ReadOnlyCollection<SortedColumn> sortedColumns, out int totalRecordCount,
            out int searchRecordCount, string searchString)
        {
            var transferlines = AllTransferOrderLines;

            totalRecordCount = transferlines.Count;

            if (!string.IsNullOrWhiteSpace(searchString))
            {
                transferlines = transferlines.Where(c => c.trn_num.ToLower().Contains(searchString.ToLower()))
                    .ToList();
            }

            searchRecordCount = transferlines.Count;

            IOrderedEnumerable<trnitem> sortedList = null;
            foreach (var sortedColumn in sortedColumns)
            {
                switch (sortedColumn.PropertyName)
                {
                    case "trn_num":
                        sortedList = sortedList == null ? transferlines.CustomSort(sortedColumn.Direction, i => i.trn_num)
                            : sortedList.CustomSort(sortedColumn.Direction, i => i.trn_num);
                        break;
                    case "trn_line":
                        sortedList = sortedList == null ? transferlines.CustomSort(sortedColumn.Direction, i => i.trn_line)
                            : sortedList.CustomSort(sortedColumn.Direction, i => i.trn_line);
                        break;
                    case "stat":
                        sortedList = sortedList == null ? transferlines.CustomSort(sortedColumn.Direction, i => i.stat)
                            : sortedList.CustomSort(sortedColumn.Direction, i => i.stat);
                        break;
                    case "item":
                        sortedList = sortedList == null ? transferlines.CustomSort(sortedColumn.Direction, i => i.item)
                            : sortedList.CustomSort(sortedColumn.Direction, i => i.item);
                        break;
                    case "trn_loc":
                        sortedList = sortedList == null ? transferlines.CustomSort(sortedColumn.Direction, i => i.trn_loc)
                            : sortedList.CustomSort(sortedColumn.Direction, i => i.trn_loc);
                        break;
                    case "ship_date":
                        sortedList = sortedList == null ? transferlines.CustomSort(sortedColumn.Direction, i => i.ship_date)
                            : sortedList.CustomSort(sortedColumn.Direction, i => i.ship_date);
                        break;
                    case "rcvd_date":
                        sortedList = sortedList == null ? transferlines.CustomSort(sortedColumn.Direction, i => i.rcvd_date)
                            : sortedList.CustomSort(sortedColumn.Direction, i => i.rcvd_date);
                        break;
                    case "qty_req":
                        sortedList = sortedList == null ? transferlines.CustomSort(sortedColumn.Direction, i => i.qty_req)
                            : sortedList.CustomSort(sortedColumn.Direction, i => i.qty_req);
                        break;
                    case "qty_shipped":
                        sortedList = sortedList == null ? transferlines.CustomSort(sortedColumn.Direction, i => i.qty_shipped)
                            : sortedList.CustomSort(sortedColumn.Direction, i => i.qty_shipped);
                        break;
                    case "qty_received":
                        sortedList = sortedList == null ? transferlines.CustomSort(sortedColumn.Direction, i => i.qty_received)
                            : sortedList.CustomSort(sortedColumn.Direction, i => i.qty_received);
                        break;
                    case "qty_loss":
                        sortedList = sortedList == null ? transferlines.CustomSort(sortedColumn.Direction, i => i.qty_loss)
                            : sortedList.CustomSort(sortedColumn.Direction, i => i.qty_loss);
                        break;
                    case "qty_packed":
                        sortedList = sortedList == null ? transferlines.CustomSort(sortedColumn.Direction, i => i.qty_packed)
                            : sortedList.CustomSort(sortedColumn.Direction, i => i.qty_packed);
                        break;
                    case "pick_date":
                        sortedList = sortedList == null ? transferlines.CustomSort(sortedColumn.Direction, i => i.pick_date)
                            : sortedList.CustomSort(sortedColumn.Direction, i => i.pick_date);
                        break;
                    case "u_m":
                        sortedList = sortedList == null ? transferlines.CustomSort(sortedColumn.Direction, i => i.u_m)
                            : sortedList.CustomSort(sortedColumn.Direction, i => i.u_m);
                        break;
                    case "sch_rcv_date":
                        sortedList = sortedList == null ? transferlines.CustomSort(sortedColumn.Direction, i => i.sch_rcv_date)
                            : sortedList.CustomSort(sortedColumn.Direction, i => i.sch_rcv_date);
                        break;
                    case "sch_ship_date":
                        sortedList = sortedList == null ? transferlines.CustomSort(sortedColumn.Direction, i => i.sch_ship_date)
                            : sortedList.CustomSort(sortedColumn.Direction, i => i.sch_ship_date);
                        break;
                    case "from_whse":
                        sortedList = sortedList == null ? transferlines.CustomSort(sortedColumn.Direction, i => i.from_whse)
                            : sortedList.CustomSort(sortedColumn.Direction, i => i.from_whse);
                        break;
                    case "to_whse":
                        sortedList = sortedList == null ? transferlines.CustomSort(sortedColumn.Direction, i => i.to_whse)
                            : sortedList.CustomSort(sortedColumn.Direction, i => i.to_whse);
                        break;
                    case "unit_weight":
                        sortedList = sortedList == null ? transferlines.CustomSort(sortedColumn.Direction, i => i.unit_weight)
                            : sortedList.CustomSort(sortedColumn.Direction, i => i.unit_weight);
                        break;
                    case "projected_date":
                        sortedList = sortedList == null ? transferlines.CustomSort(sortedColumn.Direction, i => i.projected_date)
                            : sortedList.CustomSort(sortedColumn.Direction, i => i.projected_date);
                        break;
                    case "RecordDate":
                        sortedList = sortedList == null ? transferlines.CustomSort(sortedColumn.Direction, i => i.RecordDate)
                            : sortedList.CustomSort(sortedColumn.Direction, i => i.RecordDate);
                        break;
                    case "CreatedBy":
                        sortedList = sortedList == null ? transferlines.CustomSort(sortedColumn.Direction, i => i.CreatedBy)
                            : sortedList.CustomSort(sortedColumn.Direction, i => i.CreatedBy);
                        break;
                    case "UpdatedBy":
                        sortedList = sortedList == null ? transferlines.CustomSort(sortedColumn.Direction, i => i.UpdatedBy)
                            : sortedList.CustomSort(sortedColumn.Direction, i => i.UpdatedBy);
                        break;
                    case "CreateDate":
                        sortedList = sortedList == null ? transferlines.CustomSort(sortedColumn.Direction, i => i.CreateDate)
                            : sortedList.CustomSort(sortedColumn.Direction, i => i.CreateDate);
                        break;
                }
            }

            if (pageSize == -1) //pagination is disabled in the javascript
                try { return sortedList.Skip(startIndex).ToList(); }
                catch (ArgumentNullException) { return transferlines.Skip(startIndex).ToList(); }
            else //pagination is enabled
                try { return sortedList.Skip(startIndex).Take(pageSize).ToList(); }
                catch (ArgumentNullException) { return transferlines.Skip(startIndex).Take(pageSize).ToList(); }
        }
    }

    public static class InMemoryItemWhsesRepository
    {
        public static IList<itemwhse> AllItemWhses { get; set; }

        public static IList<itemwhse> GetItemWhses(int startIndex, int pageSize, ReadOnlyCollection<SortedColumn> sortedColumns, out int totalRecordCount,
            out int searchRecordCount, string searchString)
        {
            var itemwhses = AllItemWhses;

            totalRecordCount = itemwhses.Count;

            if (!string.IsNullOrWhiteSpace(searchString))
            {
                itemwhses = itemwhses.Where(c => c.item.ToLower().Contains(searchString.ToLower()))
                    .ToList();
            }

            searchRecordCount = itemwhses.Count;

            IOrderedEnumerable<itemwhse> sortedList = null;
            foreach (var sortedColumn in sortedColumns)
            {
                switch (sortedColumn.PropertyName)
                {
                    case "item":
                        sortedList = sortedList == null ? itemwhses.CustomSort(sortedColumn.Direction, i => i.item)
                            : sortedList.CustomSort(sortedColumn.Direction, i => i.item);
                        break;
                    case "whse":
                        sortedList = sortedList == null ? itemwhses.CustomSort(sortedColumn.Direction, i => i.whse)
                            : sortedList.CustomSort(sortedColumn.Direction, i => i.whse);
                        break;
                    case "qty_on_hand":
                        sortedList = sortedList == null ? itemwhses.CustomSort(sortedColumn.Direction, i => i.qty_on_hand)
                            : sortedList.CustomSort(sortedColumn.Direction, i => i.qty_on_hand);
                        break;
                    case "qty_alloc_co":
                        sortedList = sortedList == null ? itemwhses.CustomSort(sortedColumn.Direction, i => i.qty_alloc_co)
                            : sortedList.CustomSort(sortedColumn.Direction, i => i.qty_alloc_co);
                        break;
                    case "qty_trans":
                        sortedList = sortedList == null ? itemwhses.CustomSort(sortedColumn.Direction, i => i.qty_trans)
                            : sortedList.CustomSort(sortedColumn.Direction, i => i.qty_trans);
                        break;
                }
            }
            /*I had a problem in my Datatable when adding the drill down functionality, because when I added a null column to hold my drill down image, this passed a null value to 'Sort' which caused an exception here.
             * I fixed that by adding the try catch loop below.
             */
            try { return sortedList.Skip(startIndex).Take(pageSize).ToList(); }
            catch (ArgumentNullException)
            { return itemwhses.Skip(startIndex).Take(pageSize).ToList(); }
        }
    }

    public static class InMemoryCustomerOrdersRepository
    {
        public static IList<co> AllCustomerOrders { get; set; }

        public static IList<co> GetCustomerOrders(int startIndex, int pageSize, ReadOnlyCollection<SortedColumn> sortedColumns, out int totalRecordCount,
            out int searchRecordCount, string searchString)
        {
            var customerorders = AllCustomerOrders;

            totalRecordCount = customerorders.Count;

            if (!string.IsNullOrWhiteSpace(searchString))
            {
                customerorders = customerorders.Where(c => c.co_num.ToLower().Contains(searchString.ToLower()))
                    .ToList();
            }

            searchRecordCount = customerorders.Count;

            IOrderedEnumerable<co> sortedList = null;
            foreach (var sortedColumn in sortedColumns)
            {
                switch (sortedColumn.PropertyName)
                {
                    case "type":
                        sortedList = sortedList == null ? customerorders.CustomSort(sortedColumn.Direction, i => i.type)
                            : sortedList.CustomSort(sortedColumn.Direction, i => i.type);
                        break;
                    case "co_num":
                        sortedList = sortedList == null ? customerorders.CustomSort(sortedColumn.Direction, i => i.co_num)
                            : sortedList.CustomSort(sortedColumn.Direction, i => i.co_num);
                        break;
                    case "cust_num":
                        sortedList = sortedList == null ? customerorders.CustomSort(sortedColumn.Direction, i => i.cust_num)
                            : sortedList.CustomSort(sortedColumn.Direction, i => i.cust_num);
                        break;
                    case "cust_po":
                        sortedList = sortedList == null ? customerorders.CustomSort(sortedColumn.Direction, i => i.cust_po)
                            : sortedList.CustomSort(sortedColumn.Direction, i => i.cust_po);
                        break;
                    case "order_date":
                        sortedList = sortedList == null ? customerorders.CustomSort(sortedColumn.Direction, i => i.order_date)
                            : sortedList.CustomSort(sortedColumn.Direction, i => i.order_date);
                        break;
                    case "taken_by":
                        sortedList = sortedList == null ? customerorders.CustomSort(sortedColumn.Direction, i => i.taken_by)
                            : sortedList.CustomSort(sortedColumn.Direction, i => i.taken_by);
                        break;
                    case "weight":
                        sortedList = sortedList == null ? customerorders.CustomSort(sortedColumn.Direction, i => i.weight)
                            : sortedList.CustomSort(sortedColumn.Direction, i => i.weight);
                        break;
                    case "qty_packages":
                        sortedList = sortedList == null ? customerorders.CustomSort(sortedColumn.Direction, i => i.qty_packages)
                            : sortedList.CustomSort(sortedColumn.Direction, i => i.qty_packages);
                        break;
                    case "slsman":
                        sortedList = sortedList == null ? customerorders.CustomSort(sortedColumn.Direction, i => i.slsman)
                            : sortedList.CustomSort(sortedColumn.Direction, i => i.slsman);
                        break;
                    case "eff_date":
                        sortedList = sortedList == null ? customerorders.CustomSort(sortedColumn.Direction, i => i.eff_date)
                            : sortedList.CustomSort(sortedColumn.Direction, i => i.eff_date);
                        break;
                    case "exp_date":
                        sortedList = sortedList == null ? customerorders.CustomSort(sortedColumn.Direction, i => i.exp_date)
                            : sortedList.CustomSort(sortedColumn.Direction, i => i.exp_date);
                        break;
                    case "whse":
                        sortedList = sortedList == null ? customerorders.CustomSort(sortedColumn.Direction, i => i.whse)
                            : sortedList.CustomSort(sortedColumn.Direction, i => i.whse);
                        break;
                    case "ship_partial":
                        sortedList = sortedList == null ? customerorders.CustomSort(sortedColumn.Direction, i => i.ship_partial)
                            : sortedList.CustomSort(sortedColumn.Direction, i => i.ship_partial);
                        break;
                    case "ship_early":
                        sortedList = sortedList == null ? customerorders.CustomSort(sortedColumn.Direction, i => i.ship_early)
                            : sortedList.CustomSort(sortedColumn.Direction, i => i.ship_early);
                        break;
                    case "projected_date":
                        sortedList = sortedList == null ? customerorders.CustomSort(sortedColumn.Direction, i => i.projected_date)
                            : sortedList.CustomSort(sortedColumn.Direction, i => i.projected_date);
                        break;
                    case "RecordDate":
                        sortedList = sortedList == null ? customerorders.CustomSort(sortedColumn.Direction, i => i.RecordDate)
                            : sortedList.CustomSort(sortedColumn.Direction, i => i.RecordDate);
                        break;
                    case "CreatedBy":
                        sortedList = sortedList == null ? customerorders.CustomSort(sortedColumn.Direction, i => i.CreatedBy)
                            : sortedList.CustomSort(sortedColumn.Direction, i => i.CreatedBy);
                        break;
                    case "UpdatedBy":
                        sortedList = sortedList == null ? customerorders.CustomSort(sortedColumn.Direction, i => i.UpdatedBy)
                            : sortedList.CustomSort(sortedColumn.Direction, i => i.UpdatedBy);
                        break;
                    case "CreateDate":
                        sortedList = sortedList == null ? customerorders.CustomSort(sortedColumn.Direction, i => i.CreateDate)
                            : sortedList.CustomSort(sortedColumn.Direction, i => i.CreateDate);
                        break;

                }
            }

            /*I had a problem in my Datatable when adding the drill down functionality, because when I added a null column to hold my drill down image, this passed a null value to 'Sort' which caused an exception here.
             * I fixed that by adding the try catch loop below.
             */
            try{ return sortedList.Skip(startIndex).Take(pageSize).ToList();}
            catch(ArgumentNullException) 
            {return customerorders.Skip(startIndex).Take(pageSize).ToList();}
            
        }
    }

    public static class InMemoryCustomerOrderLinesRepository
    {
        public static IList<coitem> AllCustomerOrderLines { get; set; }

        public static IList<coitem> GetCustomerOrderLines(int startIndex, int pageSize, ReadOnlyCollection<SortedColumn> sortedColumns, out int totalRecordCount,
            out int searchRecordCount, string searchString)
        {
            var customerorderlines = AllCustomerOrderLines;

            totalRecordCount = customerorderlines.Count;

            if (!string.IsNullOrWhiteSpace(searchString))
            {
                customerorderlines = customerorderlines.Where(c => c.co_num.ToLower().Contains(searchString.ToLower()))
                    .ToList();
            }

            searchRecordCount = customerorderlines.Count;

            IOrderedEnumerable<coitem> sortedList = null;
            foreach (var sortedColumn in sortedColumns)
            {
                switch (sortedColumn.PropertyName)
                {
                    case "co_num":
                        sortedList = sortedList == null ? customerorderlines.CustomSort(sortedColumn.Direction, i => i.co_num)
                            : sortedList.CustomSort(sortedColumn.Direction, i => i.co_num);
                        break;
                    case "co_line":
                        sortedList = sortedList == null ? customerorderlines.CustomSort(sortedColumn.Direction, i => i.co_line)
                            : sortedList.CustomSort(sortedColumn.Direction, i => i.co_line);
                        break;
                    case "co_release":
                        sortedList = sortedList == null ? customerorderlines.CustomSort(sortedColumn.Direction, i => i.co_release)
                            : sortedList.CustomSort(sortedColumn.Direction, i => i.co_release);
                        break;
                    case "item":
                        sortedList = sortedList == null ? customerorderlines.CustomSort(sortedColumn.Direction, i => i.item)
                            : sortedList.CustomSort(sortedColumn.Direction, i => i.item);
                        break;
                    case "qty_ordered":
                        sortedList = sortedList == null ? customerorderlines.CustomSort(sortedColumn.Direction, i => i.qty_ordered)
                            : sortedList.CustomSort(sortedColumn.Direction, i => i.qty_ordered);
                        break;
                    case "qty_ready":
                        sortedList = sortedList == null ? customerorderlines.CustomSort(sortedColumn.Direction, i => i.qty_ready)
                            : sortedList.CustomSort(sortedColumn.Direction, i => i.qty_ready);
                        break;
                    case "qty_shipped":
                        sortedList = sortedList == null ? customerorderlines.CustomSort(sortedColumn.Direction, i => i.qty_shipped)
                            : sortedList.CustomSort(sortedColumn.Direction, i => i.qty_shipped);
                        break;
                    case "qty_packed":
                        sortedList = sortedList == null ? customerorderlines.CustomSort(sortedColumn.Direction, i => i.qty_packed)
                            : sortedList.CustomSort(sortedColumn.Direction, i => i.qty_packed);
                        break;
                    case "ref_type":
                        sortedList = sortedList == null ? customerorderlines.CustomSort(sortedColumn.Direction, i => i.ref_type)
                            : sortedList.CustomSort(sortedColumn.Direction, i => i.ref_type);
                        break;
                    case "ref_num":
                        sortedList = sortedList == null ? customerorderlines.CustomSort(sortedColumn.Direction, i => i.ref_num)
                            : sortedList.CustomSort(sortedColumn.Direction, i => i.ref_num);
                        break;
                    case "ref_line_suf":
                        sortedList = sortedList == null ? customerorderlines.CustomSort(sortedColumn.Direction, i => i.ref_line_suf)
                            : sortedList.CustomSort(sortedColumn.Direction, i => i.ref_line_suf);
                        break;
                    case "ref_release":
                        sortedList = sortedList == null ? customerorderlines.CustomSort(sortedColumn.Direction, i => i.ref_release)
                            : sortedList.CustomSort(sortedColumn.Direction, i => i.ref_release);
                        break;
                    case "due_date":
                        sortedList = sortedList == null ? customerorderlines.CustomSort(sortedColumn.Direction, i => i.due_date)
                            : sortedList.CustomSort(sortedColumn.Direction, i => i.due_date);
                        break;
                    case "cust_item":
                        sortedList = sortedList == null ? customerorderlines.CustomSort(sortedColumn.Direction, i => i.cust_item)
                            : sortedList.CustomSort(sortedColumn.Direction, i => i.cust_item);
                        break;
                    case "qty_invoiced":
                        sortedList = sortedList == null ? customerorderlines.CustomSort(sortedColumn.Direction, i => i.qty_invoiced)
                            : sortedList.CustomSort(sortedColumn.Direction, i => i.qty_invoiced);
                        break;
                    case "qty_returned":
                        sortedList = sortedList == null ? customerorderlines.CustomSort(sortedColumn.Direction, i => i.qty_returned)
                            : sortedList.CustomSort(sortedColumn.Direction, i => i.qty_returned);
                        break;
                    case "stat":
                        sortedList = sortedList == null ? customerorderlines.CustomSort(sortedColumn.Direction, i => i.stat)
                            : sortedList.CustomSort(sortedColumn.Direction, i => i.stat);
                        break;
                    case "cust_num":
                        sortedList = sortedList == null ? customerorderlines.CustomSort(sortedColumn.Direction, i => i.cust_num)
                            : sortedList.CustomSort(sortedColumn.Direction, i => i.cust_num);
                        break;
                    case "cust_seq":
                        sortedList = sortedList == null ? customerorderlines.CustomSort(sortedColumn.Direction, i => i.cust_seq)
                            : sortedList.CustomSort(sortedColumn.Direction, i => i.cust_seq);
                        break;
                    case "release_date":
                        sortedList = sortedList == null ? customerorderlines.CustomSort(sortedColumn.Direction, i => i.release_date)
                            : sortedList.CustomSort(sortedColumn.Direction, i => i.release_date);
                        break;
                    case "promise_date":
                        sortedList = sortedList == null ? customerorderlines.CustomSort(sortedColumn.Direction, i => i.promise_date)
                            : sortedList.CustomSort(sortedColumn.Direction, i => i.promise_date);
                        break;
                    case "whse":
                        sortedList = sortedList == null ? customerorderlines.CustomSort(sortedColumn.Direction, i => i.whse)
                            : sortedList.CustomSort(sortedColumn.Direction, i => i.whse);
                        break;
                    case "unit_weight":
                        sortedList = sortedList == null ? customerorderlines.CustomSort(sortedColumn.Direction, i => i.unit_weight)
                            : sortedList.CustomSort(sortedColumn.Direction, i => i.unit_weight);
                        break;
                    case "pick_date":
                        sortedList = sortedList == null ? customerorderlines.CustomSort(sortedColumn.Direction, i => i.pick_date)
                            : sortedList.CustomSort(sortedColumn.Direction, i => i.pick_date);
                        break;
                    case "u_m":
                        sortedList = sortedList == null ? customerorderlines.CustomSort(sortedColumn.Direction, i => i.u_m)
                            : sortedList.CustomSort(sortedColumn.Direction, i => i.u_m);
                        break;
                    case "co_cust_num":
                        sortedList = sortedList == null ? customerorderlines.CustomSort(sortedColumn.Direction, i => i.co_cust_num)
                            : sortedList.CustomSort(sortedColumn.Direction, i => i.co_cust_num);
                        break;
                    case "cust_po":
                        sortedList = sortedList == null ? customerorderlines.CustomSort(sortedColumn.Direction, i => i.cust_po)
                            : sortedList.CustomSort(sortedColumn.Direction, i => i.cust_po);
                        break;
                    case "projected_date":
                        sortedList = sortedList == null ? customerorderlines.CustomSort(sortedColumn.Direction, i => i.projected_date)
                            : sortedList.CustomSort(sortedColumn.Direction, i => i.projected_date);
                        break;
                    case "RecordDate":
                        sortedList = sortedList == null ? customerorderlines.CustomSort(sortedColumn.Direction, i => i.RecordDate)
                            : sortedList.CustomSort(sortedColumn.Direction, i => i.RecordDate);
                        break;
                    case "description":
                        sortedList = sortedList == null ? customerorderlines.CustomSort(sortedColumn.Direction, i => i.description)
                            : sortedList.CustomSort(sortedColumn.Direction, i => i.description);
                        break;
                    case "CreatedBy":
                        sortedList = sortedList == null ? customerorderlines.CustomSort(sortedColumn.Direction, i => i.CreatedBy)
                            : sortedList.CustomSort(sortedColumn.Direction, i => i.CreatedBy);
                        break;
                    case "UpdatedBy":
                        sortedList = sortedList == null ? customerorderlines.CustomSort(sortedColumn.Direction, i => i.UpdatedBy)
                            : sortedList.CustomSort(sortedColumn.Direction, i => i.UpdatedBy);
                        break;
                    case "CreateDate":
                        sortedList = sortedList == null ? customerorderlines.CustomSort(sortedColumn.Direction, i => i.CreateDate)
                            : sortedList.CustomSort(sortedColumn.Direction, i => i.CreateDate);
                        break;
                }
            }

            /*I had a problem in my Datatable when adding the drill down functionality, because when I added a null column to hold my drill down image, this passed a null value to 'Sort' which caused an exception here.
             * I fixed that by adding the try catch loop below.
             * UPDATE - if paging is disabled in the javascript (see Views/CustomerOrders/Search.cshtml for orderline tInnerTable that has '"bPaginate": false' attribute set), then the JQueryDataTablesModel enums iDisplayLength
             *  property gets set to -1. However, this will result in no records being returned based on the below statements. So to allow pagination to be disabled and still have records returned, I added the below if statement
             *  to check for the condition.
             */
            if (pageSize == -1) //pagination is disabled in the javascript
                try { return sortedList.Skip(startIndex).ToList(); }
                catch (ArgumentNullException) { return customerorderlines.Skip(startIndex).ToList(); }
            else //pagination is enabled
                try { return sortedList.Skip(startIndex).Take(pageSize).ToList(); }
                catch (ArgumentNullException) { return customerorderlines.Skip(startIndex).Take(pageSize).ToList(); }
            

        }
    }

    public static class InMemoryPurchaseOrdersRepository
    {
        public static IList<po> AllPurchaseOrders { get; set; }

        public static IList<po> GetPurchaseOrders(int startIndex, int pageSize, ReadOnlyCollection<SortedColumn> sortedColumns, out int totalRecordCount,
            out int searchRecordCount, string searchString)
        {
            var purchaseorders = AllPurchaseOrders;

            totalRecordCount = purchaseorders.Count;

            if (!string.IsNullOrWhiteSpace(searchString))
            {
                purchaseorders = purchaseorders.Where(c => c.po_num.ToLower().Contains(searchString.ToLower()))
                    .ToList();
            }

            searchRecordCount = purchaseorders.Count;

            IOrderedEnumerable<po> sortedList = null;
            foreach (var sortedColumn in sortedColumns)
            {
                switch (sortedColumn.PropertyName)
                {
                    case "po_num":
                        sortedList = sortedList == null ? purchaseorders.CustomSort(sortedColumn.Direction, i => i.po_num)
                            : sortedList.CustomSort(sortedColumn.Direction, i => i.po_num);
                        break;
                    case "vend_num":
                        sortedList = sortedList == null ? purchaseorders.CustomSort(sortedColumn.Direction, i => i.vend_num)
                            : sortedList.CustomSort(sortedColumn.Direction, i => i.vend_num);
                        break;
                    case "order_date":
                        sortedList = sortedList == null ? purchaseorders.CustomSort(sortedColumn.Direction, i => i.order_date)
                            : sortedList.CustomSort(sortedColumn.Direction, i => i.order_date);
                        break;
                    case "ship_code":
                        sortedList = sortedList == null ? purchaseorders.CustomSort(sortedColumn.Direction, i => i.ship_code)
                            : sortedList.CustomSort(sortedColumn.Direction, i => i.ship_code);
                        break;
                    case "terms_code":
                        sortedList = sortedList == null ? purchaseorders.CustomSort(sortedColumn.Direction, i => i.terms_code)
                            : sortedList.CustomSort(sortedColumn.Direction, i => i.terms_code);
                        break;
                    case "fob":
                        sortedList = sortedList == null ? purchaseorders.CustomSort(sortedColumn.Direction, i => i.fob)
                            : sortedList.CustomSort(sortedColumn.Direction, i => i.fob);
                        break;
                    case "inv_date":
                        sortedList = sortedList == null ? purchaseorders.CustomSort(sortedColumn.Direction, i => i.inv_date)
                            : sortedList.CustomSort(sortedColumn.Direction, i => i.inv_date);
                        break;
                    case "inv_num":
                        sortedList = sortedList == null ? purchaseorders.CustomSort(sortedColumn.Direction, i => i.inv_num)
                            : sortedList.CustomSort(sortedColumn.Direction, i => i.inv_num);
                        break;
                    case "type":
                        sortedList = sortedList == null ? purchaseorders.CustomSort(sortedColumn.Direction, i => i.type)
                            : sortedList.CustomSort(sortedColumn.Direction, i => i.type);
                        break;
                    case "drop_ship_no":
                        sortedList = sortedList == null ? purchaseorders.CustomSort(sortedColumn.Direction, i => i.drop_ship_no)
                            : sortedList.CustomSort(sortedColumn.Direction, i => i.drop_ship_no);
                        break;
                    case "drop_seq":
                        sortedList = sortedList == null ? purchaseorders.CustomSort(sortedColumn.Direction, i => i.drop_seq)
                            : sortedList.CustomSort(sortedColumn.Direction, i => i.drop_seq);
                        break;
                    case "eff_date":
                        sortedList = sortedList == null ? purchaseorders.CustomSort(sortedColumn.Direction, i => i.eff_date)
                            : sortedList.CustomSort(sortedColumn.Direction, i => i.eff_date);
                        break;
                    case "exp_date":
                        sortedList = sortedList == null ? purchaseorders.CustomSort(sortedColumn.Direction, i => i.exp_date)
                            : sortedList.CustomSort(sortedColumn.Direction, i => i.exp_date);
                        break;
                    case "ship_addr":
                        sortedList = sortedList == null ? purchaseorders.CustomSort(sortedColumn.Direction, i => i.ship_addr)
                            : sortedList.CustomSort(sortedColumn.Direction, i => i.ship_addr);
                        break;
                    case "whse":
                        sortedList = sortedList == null ? purchaseorders.CustomSort(sortedColumn.Direction, i => i.whse)
                            : sortedList.CustomSort(sortedColumn.Direction, i => i.whse);
                        break;
                    case "RecordDate":
                        sortedList = sortedList == null ? purchaseorders.CustomSort(sortedColumn.Direction, i => i.RecordDate)
                            : sortedList.CustomSort(sortedColumn.Direction, i => i.RecordDate);
                        break;
                    case "buyer":
                        sortedList = sortedList == null ? purchaseorders.CustomSort(sortedColumn.Direction, i => i.buyer)
                            : sortedList.CustomSort(sortedColumn.Direction, i => i.buyer);
                        break;
                    case "CreatedBy":
                        sortedList = sortedList == null ? purchaseorders.CustomSort(sortedColumn.Direction, i => i.CreatedBy)
                            : sortedList.CustomSort(sortedColumn.Direction, i => i.CreatedBy);
                        break;
                    case "UpdatedBy":
                        sortedList = sortedList == null ? purchaseorders.CustomSort(sortedColumn.Direction, i => i.UpdatedBy)
                            : sortedList.CustomSort(sortedColumn.Direction, i => i.UpdatedBy);
                        break;
                    case "CreateDate":
                        sortedList = sortedList == null ? purchaseorders.CustomSort(sortedColumn.Direction, i => i.CreateDate)
                            : sortedList.CustomSort(sortedColumn.Direction, i => i.CreateDate);
                        break;

                }
            }
            /*I had a problem in my Datatable when adding the drill down functionality, because when I added a null column to hold my drill down image, this passed a null value to 'Sort' which caused an exception here.
             * I fixed that by adding the try catch loop below.
             */
            try { return sortedList.Skip(startIndex).Take(pageSize).ToList(); }
            catch (ArgumentNullException)
            { return purchaseorders.Skip(startIndex).Take(pageSize).ToList(); }
        }
    }

    public static class InMemoryPurchaseOrderLinesRepository
    {
        public static IList<poitem> AllPurchaseOrderLines { get; set; }

        public static IList<poitem> GetPurchaseOrderLines(int startIndex, int pageSize, ReadOnlyCollection<SortedColumn> sortedColumns, out int totalRecordCount,
            out int searchRecordCount, string searchString)
        {
            var purchaseorderlines = AllPurchaseOrderLines;

            totalRecordCount = purchaseorderlines.Count;

            if (!string.IsNullOrWhiteSpace(searchString))
            {
                purchaseorderlines = purchaseorderlines.Where(c => c.po_num.ToLower().Contains(searchString.ToLower()))
                    .ToList();
            }

            searchRecordCount = purchaseorderlines.Count;

            IOrderedEnumerable<poitem> sortedList = null;
            foreach (var sortedColumn in sortedColumns)
            {
                switch (sortedColumn.PropertyName)
                {
                    case "po_num":
                        sortedList = sortedList == null ? purchaseorderlines.CustomSort(sortedColumn.Direction, i => i.po_num)
                            : sortedList.CustomSort(sortedColumn.Direction, i => i.po_num);
                        break;
                    case "po_line":
                        sortedList = sortedList == null ? purchaseorderlines.CustomSort(sortedColumn.Direction, i => i.po_line)
                            : sortedList.CustomSort(sortedColumn.Direction, i => i.po_line);
                        break;
                    case "po_release":
                        sortedList = sortedList == null ? purchaseorderlines.CustomSort(sortedColumn.Direction, i => i.po_release)
                            : sortedList.CustomSort(sortedColumn.Direction, i => i.po_release);
                        break;
                    case "item":
                        sortedList = sortedList == null ? purchaseorderlines.CustomSort(sortedColumn.Direction, i => i.item)
                            : sortedList.CustomSort(sortedColumn.Direction, i => i.item);
                        break;
                    case "stat":
                        sortedList = sortedList == null ? purchaseorderlines.CustomSort(sortedColumn.Direction, i => i.stat)
                            : sortedList.CustomSort(sortedColumn.Direction, i => i.stat);
                        break;
                    case "qty_ordered":
                        sortedList = sortedList == null ? purchaseorderlines.CustomSort(sortedColumn.Direction, i => i.qty_ordered)
                            : sortedList.CustomSort(sortedColumn.Direction, i => i.qty_ordered);
                        break;
                    case "qty_received":
                        sortedList = sortedList == null ? purchaseorderlines.CustomSort(sortedColumn.Direction, i => i.qty_received)
                            : sortedList.CustomSort(sortedColumn.Direction, i => i.qty_received);
                        break;
                    case "qty_rejected":
                        sortedList = sortedList == null ? purchaseorderlines.CustomSort(sortedColumn.Direction, i => i.qty_rejected)
                            : sortedList.CustomSort(sortedColumn.Direction, i => i.qty_rejected);
                        break;
                    case "qty_voucher":
                        sortedList = sortedList == null ? purchaseorderlines.CustomSort(sortedColumn.Direction, i => i.qty_voucher)
                            : sortedList.CustomSort(sortedColumn.Direction, i => i.qty_voucher);
                        break;
                    case "qty_returned":
                        sortedList = sortedList == null ? purchaseorderlines.CustomSort(sortedColumn.Direction, i => i.qty_returned)
                            : sortedList.CustomSort(sortedColumn.Direction, i => i.qty_returned);
                        break;
                    case "due_date":
                        sortedList = sortedList == null ? purchaseorderlines.CustomSort(sortedColumn.Direction, i => i.due_date)
                            : sortedList.CustomSort(sortedColumn.Direction, i => i.due_date);
                        break;
                    case "rcvd_date":
                        sortedList = sortedList == null ? purchaseorderlines.CustomSort(sortedColumn.Direction, i => i.rcvd_date)
                            : sortedList.CustomSort(sortedColumn.Direction, i => i.rcvd_date);
                        break;
                    case "vend_item":
                        sortedList = sortedList == null ? purchaseorderlines.CustomSort(sortedColumn.Direction, i => i.vend_item)
                            : sortedList.CustomSort(sortedColumn.Direction, i => i.vend_item);
                        break;
                    case "ship_addr":
                        sortedList = sortedList == null ? purchaseorderlines.CustomSort(sortedColumn.Direction, i => i.ship_addr)
                            : sortedList.CustomSort(sortedColumn.Direction, i => i.ship_addr);
                        break;
                    case "promise_date":
                        sortedList = sortedList == null ? purchaseorderlines.CustomSort(sortedColumn.Direction, i => i.promise_date)
                            : sortedList.CustomSort(sortedColumn.Direction, i => i.promise_date);
                        break;
                    case "release_date":
                        sortedList = sortedList == null ? purchaseorderlines.CustomSort(sortedColumn.Direction, i => i.release_date)
                            : sortedList.CustomSort(sortedColumn.Direction, i => i.release_date);
                        break;
                    case "whse":
                        sortedList = sortedList == null ? purchaseorderlines.CustomSort(sortedColumn.Direction, i => i.whse)
                            : sortedList.CustomSort(sortedColumn.Direction, i => i.whse);
                        break;
                    case "unit_weight":
                        sortedList = sortedList == null ? purchaseorderlines.CustomSort(sortedColumn.Direction, i => i.unit_weight)
                            : sortedList.CustomSort(sortedColumn.Direction, i => i.unit_weight);
                        break;
                    case "u_m":
                        sortedList = sortedList == null ? purchaseorderlines.CustomSort(sortedColumn.Direction, i => i.u_m)
                            : sortedList.CustomSort(sortedColumn.Direction, i => i.u_m);
                        break;
                    case "revision":
                        sortedList = sortedList == null ? purchaseorderlines.CustomSort(sortedColumn.Direction, i => i.revision)
                            : sortedList.CustomSort(sortedColumn.Direction, i => i.revision);
                        break;
                    case "drawing_nbr":
                        sortedList = sortedList == null ? purchaseorderlines.CustomSort(sortedColumn.Direction, i => i.drawing_nbr)
                            : sortedList.CustomSort(sortedColumn.Direction, i => i.drawing_nbr);
                        break;
                    case "po_vend_num":
                        sortedList = sortedList == null ? purchaseorderlines.CustomSort(sortedColumn.Direction, i => i.po_vend_num)
                            : sortedList.CustomSort(sortedColumn.Direction, i => i.po_vend_num);
                        break;
                    case "RecordDate":
                        sortedList = sortedList == null ? purchaseorderlines.CustomSort(sortedColumn.Direction, i => i.RecordDate)
                            : sortedList.CustomSort(sortedColumn.Direction, i => i.RecordDate);
                        break;
                    case "description":
                        sortedList = sortedList == null ? purchaseorderlines.CustomSort(sortedColumn.Direction, i => i.description)
                            : sortedList.CustomSort(sortedColumn.Direction, i => i.description);
                        break;
                    case "CreatedBy":
                        sortedList = sortedList == null ? purchaseorderlines.CustomSort(sortedColumn.Direction, i => i.CreatedBy)
                            : sortedList.CustomSort(sortedColumn.Direction, i => i.CreatedBy);
                        break;
                    case "UpdatedBy":
                        sortedList = sortedList == null ? purchaseorderlines.CustomSort(sortedColumn.Direction, i => i.UpdatedBy)
                            : sortedList.CustomSort(sortedColumn.Direction, i => i.UpdatedBy);
                        break;
                    case "CreateDate":
                        sortedList = sortedList == null ? purchaseorderlines.CustomSort(sortedColumn.Direction, i => i.CreateDate)
                            : sortedList.CustomSort(sortedColumn.Direction, i => i.CreateDate);
                        break;
                    case "rcpt_rqmt":
                        sortedList = sortedList == null ? purchaseorderlines.CustomSort(sortedColumn.Direction, i => i.rcpt_rqmt)
                            : sortedList.CustomSort(sortedColumn.Direction, i => i.rcpt_rqmt);
                        break;
                    case "manufacturer_id":
                        sortedList = sortedList == null ? purchaseorderlines.CustomSort(sortedColumn.Direction, i => i.manufacturer_id)
                            : sortedList.CustomSort(sortedColumn.Direction, i => i.manufacturer_id);
                        break;
                    case "manufacturer_item":
                        sortedList = sortedList == null ? purchaseorderlines.CustomSort(sortedColumn.Direction, i => i.manufacturer_item)
                            : sortedList.CustomSort(sortedColumn.Direction, i => i.manufacturer_item);
                        break;
                }
            }

            /*I had a problem in my Datatable when adding the drill down functionality, because when I added a null column to hold my drill down image, this passed a null value to 'Sort' which caused an exception here.
             * I fixed that by adding the try catch loop below.
             * UPDATE - if paging is disabled in the javascript (see Views/CustomerOrders/Search.cshtml for orderline tInnerTable that has '"bPaginate": false' attribute set), then the JQueryDataTablesModel enums iDisplayLength
             *  property gets set to -1. However, this will result in no records being returned based on the below statements. So to allow pagination to be disabled and still have records returned, I added the below if statement
             *  to check for the condition.
             */
            if (pageSize == -1) //pagination is disabled in the javascript
                try { return sortedList.Skip(startIndex).ToList(); }
                catch (ArgumentNullException) { return purchaseorderlines.Skip(startIndex).ToList(); }
            else //pagination is enabled
                try { return sortedList.Skip(startIndex).Take(pageSize).ToList(); }
                catch (ArgumentNullException) { return purchaseorderlines.Skip(startIndex).Take(pageSize).ToList(); }


        }
    }

    public static class InMemoryNotesRepository
    {
        public static IList<SytelineNote> AllNotes { get; set; }

        public static IList<SytelineNote> GetNotes(int startIndex, int pageSize, ReadOnlyCollection<SortedColumn> sortedColumns, out int totalRecordCount,
            out int searchRecordCount, string searchString)
        {
            var notes = AllNotes;

            totalRecordCount = notes.Count;

            if (!string.IsNullOrWhiteSpace(searchString))
            {
                notes = notes.Where(c => c.NoteContent.ToLower().Contains(searchString.ToLower()))
                    .ToList();
            }

            searchRecordCount = notes.Count;

            IOrderedEnumerable<SytelineNote> sortedList = null;
            foreach (var sortedColumn in sortedColumns)
            {
                switch (sortedColumn.PropertyName)
                {
                    case "LastUpdated":
                        sortedList = sortedList == null ? notes.CustomSort(sortedColumn.Direction, i => i.LastUpdated)
                            : sortedList.CustomSort(sortedColumn.Direction, i => i.LastUpdated);
                        break;
                    case "NoteContent":
                        sortedList = sortedList == null ? notes.CustomSort(sortedColumn.Direction, i => i.NoteContent)
                            : sortedList.CustomSort(sortedColumn.Direction, i => i.NoteContent);
                        break;
                    case "NoteDesc":
                        sortedList = sortedList == null ? notes.CustomSort(sortedColumn.Direction, i => i.NoteDesc)
                            : sortedList.CustomSort(sortedColumn.Direction, i => i.NoteDesc);
                        break;
                    case "CreatedBy":
                        sortedList = sortedList == null ? notes.CustomSort(sortedColumn.Direction, i => i.CreatedBy)
                            : sortedList.CustomSort(sortedColumn.Direction, i => i.CreatedBy);
                        break;
                    case "UpdatedBy":
                        sortedList = sortedList == null ? notes.CustomSort(sortedColumn.Direction, i => i.UpdatedBy)
                            : sortedList.CustomSort(sortedColumn.Direction, i => i.UpdatedBy);
                        break;
                    case "IsInternal":
                        sortedList = sortedList == null ? notes.CustomSort(sortedColumn.Direction, i => i.IsInternal)
                            : sortedList.CustomSort(sortedColumn.Direction, i => i.IsInternal);
                        break;
                }
            }

            if (pageSize == -1) //pagination is disabled in the javascript
                try { return sortedList.Skip(startIndex).ToList(); }
                catch (ArgumentNullException) { return notes.Skip(startIndex).ToList(); }
            else //pagination is enabled
                try { return sortedList.Skip(startIndex).Take(pageSize).ToList(); }
                catch (ArgumentNullException) { return notes.Skip(startIndex).Take(pageSize).ToList(); }


        }
    }
}