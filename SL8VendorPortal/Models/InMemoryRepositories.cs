using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using System.Collections.ObjectModel;
using jQuery.DataTables.Mvc;
using SL8VendorPortal.Infrastructure;
using System.Data.Objects.SqlClient;


namespace SL8VendorPortal.Models
{

    public static class InMemoryTransferOrdersRepository
    {
        public static IList<transfer> AllTransferOrders { get; set; }

        public static IList<transfer> GetTransferOrders(int startIndex, int pageSize, ReadOnlyCollection<SortedColumn> sortedColumns, out int totalRecordCount,
            out int searchRecordCount, string searchString, bool isDownloadReport = false)
        {
            var transfers = AllTransferOrders;

            totalRecordCount = transfers.Count;

            //this was changed to accomodate the users request to search the Orders by Item which is only contained in the line/release.
            //To accomodate this, I added the line/release data to the parent order in the Search method of the controller so that I could perform the search
            //on the sub objects here...
            if (!string.IsNullOrWhiteSpace(searchString))
            {
                //transfers = transfers.Where(c => c.trn_num.ToLower().Contains(searchString.ToLower()))
                //    .ToList();
                transfers = transfers.Where(t => t.trnitems.Any(l => !string.IsNullOrEmpty(l.item) && l.item.ToLower().Contains(searchString.ToLower())))
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
                    case "RecordDate":
                        sortedList = sortedList == null ? transfers.CustomSort(sortedColumn.Direction, i => i.RecordDate)
                            : sortedList.CustomSort(sortedColumn.Direction, i => i.RecordDate);
                        break;
                    default://This took care of the below scenario where the default sorted column was 0 which is my drill down image. I could have used 'case "0":' but just made a default case instead...
                        sortedList = sortedList == null ? transfers.CustomSort(sortedColumn.Direction, i => i.trn_num)
                            : sortedList;
                        break;
                }
            }
            /*I had a problem in my Datatable when adding the drill down functionality, because when I added a null column to hold my drill down image, this passed a null value to 'Sort' which caused an exception here.
             * I fixed that by adding the try catch loop below.
             
            try { return sortedList.Skip(startIndex).Take(pageSize).ToList(); }
            catch (ArgumentNullException)
            { return transfers.Skip(startIndex).Take(pageSize).ToList(); }*/
            if (isDownloadReport)
                return sortedList.ToList();
            else
                return sortedList.Skip(startIndex).Take(pageSize).ToList();
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
            out int searchRecordCount, string searchString, bool isDownloadReport = false)
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
                    default://This took care of the below scenario where the default sorted column was 0 which is my drill down image. I could have used 'case "0":' but just made a default case instead...
                        sortedList = sortedList == null ? itemwhses.CustomSort(sortedColumn.Direction, i => i.item)
                            : sortedList;
                        break;
                }
            }
            /*I had a problem in my Datatable when adding the drill down functionality, because when I added a null column to hold my drill down image, this passed a null value to 'Sort' which caused an exception here.
             * I fixed that by adding the try catch loop below.
             
            try { return sortedList.Skip(startIndex).Take(pageSize).ToList(); }
            catch (ArgumentNullException)
            { return itemwhses.Skip(startIndex).Take(pageSize).ToList(); }*/
            if (isDownloadReport)
                return sortedList.ToList();
            else
                return sortedList.Skip(startIndex).Take(pageSize).ToList();
        }
    }

    public static class InMemoryCustomerOrdersRepository
    {
        public static IList<co> AllCustomerOrders { get; set; }

        public static IList<co> GetCustomerOrders(int startIndex, int pageSize, ReadOnlyCollection<SortedColumn> sortedColumns, out int totalRecordCount,
            out int searchRecordCount, string searchString, bool isDownloadReport = false)
        {
            var customerorders = AllCustomerOrders;

            totalRecordCount = customerorders.Count;

            //this was changed to accomodate the users request to search the Orders by Item which is only contained in the line/release.
            //To accomodate this, I added the line/release data to the parent order in the Search method of the controller so that I could perform the search
            //on the sub objects here...
            if (!string.IsNullOrWhiteSpace(searchString))
            {
                //customerorders = customerorders.Where(c => c.co_num.ToLower().Contains(searchString.ToLower()))
                //    .ToList();
                customerorders = customerorders.Where(c => c.coitems.Any(l => !string.IsNullOrEmpty(l.item) && l.item.ToLower().Contains(searchString.ToLower())))
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
                    default://This took care of the below scenario where the default sorted column was 0 which is my drill down image. I could have used 'case "0":' but just made a default case instead...
                        sortedList = sortedList == null ? customerorders.CustomSort(sortedColumn.Direction, i => i.co_num)
                            : sortedList;
                        break;
                }
            }

            /*I had a problem in my Datatable when adding the drill down functionality, because when I added a null column to hold my drill down image, this passed a null value to 'Sort' which caused an exception here.
             * I fixed that by adding the try catch loop below.
             
            try{ return sortedList.Skip(startIndex).Take(pageSize).ToList();}
            catch(ArgumentNullException) 
            {return customerorders.Skip(startIndex).Take(pageSize).ToList();}*/
            if (isDownloadReport)
                return sortedList.ToList();
            else
                return sortedList.Skip(startIndex).Take(pageSize).ToList();
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
            out int searchRecordCount, string searchString, bool isDownloadReport = false)
        {
            var purchaseorders = AllPurchaseOrders;

            totalRecordCount = purchaseorders.Count;

            //this was changed to accomodate the users request to search the Orders by Item which is only contained in the line/release.
            //To accomodate this, I added the line/release data to the parent order in the Search method of the controller so that I could perform the search
            //on the sub objects here...
            if (!string.IsNullOrWhiteSpace(searchString))
            {
                //purchaseorders = purchaseorders.Where(c => c.po_num.ToLower().Contains(searchString.ToLower()))
                //    .ToList();
                purchaseorders = purchaseorders
                    .Where(p => p.poitems.Any(l => !string.IsNullOrEmpty(l.item) && l.item.ToLower().Contains(searchString.ToLower())))
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
                    default://This took care of the below scenario where the default sorted column was 0 which is my drill down image. I could have used 'case "0":' but just made a default case instead...
                        sortedList = sortedList == null ? purchaseorders.CustomSort(sortedColumn.Direction, i => i.po_num)
                            : sortedList;
                        break;
                }
            }
            /*I had a problem in my Datatable when adding the drill down functionality, because when I added a null column to hold my drill down image, this passed a null value to 'Sort' which caused an exception here.
             * I fixed that by adding the try catch loop below.
             
            try { return sortedList.Skip(startIndex).Take(pageSize).ToList(); }
            catch (ArgumentNullException)
            { return purchaseorders.Skip(startIndex).Take(pageSize).ToList(); }*/
            if (isDownloadReport)
                return sortedList.ToList();
            else
                return sortedList.Skip(startIndex).Take(pageSize).ToList();
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

    public static class InMemoryVendorRequestsRepository
    {
        public static IList<VendorRequest> AllVendorRequests { get; set; }

        public static IList<VendorRequest> GetVendorRequests(int MaxRecordCount, out int totalRecordCount, out int searchRecordCount, JQueryDataTablesModel DataTablesModel, bool isDownloadReport = false)
        {
            SL8VendorPortalDb VendorPortalDb = new SL8VendorPortalDb();
            ReadOnlyCollection<SortedColumn> sortedColumns = DataTablesModel.GetSortedColumns();
            IList<VendorRequest> vendorrequests;
            DateTime dtmTemp;
            string[] objResults;
            VendorRequestSearch objVendorRequestSearch;


            totalRecordCount = VendorPortalDb.VendorRequests.Count();

            objVendorRequestSearch = new VendorRequestSearch();
            for (int intCounter = 0; intCounter < DataTablesModel.iColumns; intCounter++)
            {

                if (DataTablesModel.bSearchable_[intCounter] == true && !string.IsNullOrEmpty(DataTablesModel.sSearch_[intCounter]))
                {
                    /*For some reason when I implemented resizable movable columns and would then move the columns in the application, the application would send tilde's in the 'checkbox' column types sSearch field which was wierd
                     since the checkbox column types are delimited by the pipe | character and the 'range' column types are delimited by the tilde...  The resolution that I came up with was to check if the only value passed in sSearch
                     was a tilde and if it was then skip the loop so that the respective VendorRequestSearch field was left null.*/
                    if (DataTablesModel.sSearch_[intCounter].Equals("~"))
                        continue;

                    /*Notice that i had to use mDataProp2_ due to datatables multi-column filtering not placing sSearch into proper array position when columns are reordered; See VendorRequestsController.cs Search method for details...*/
                    switch (DataTablesModel.mDataProp2_[intCounter])
                    {
                        case "ID":
                            objVendorRequestSearch.ID = DataTablesModel.sSearch_[intCounter];
                            break;
                        case "Processed":
                            objResults = DataTablesModel.sSearch_[intCounter].Split('|');//results returned from a checklist are delimited by the pipe char
                            if (objResults.Length > 1)//there are two results returned so both true and false have been selected
                                objVendorRequestSearch.Processed = "Both";
                            else
                                objVendorRequestSearch.Processed = objResults[0];
                            break;
                        case "Notes":
                            break;
                        case "DateProcessed":
                            objResults = DataTablesModel.sSearch_[intCounter].Split('~');//results returned from a daterange are delimited by the tilde char
                            objVendorRequestSearch.DateProcessedGT = DateTime.TryParse(objResults[0], out dtmTemp) ? dtmTemp : DateTime.MinValue;
                            objVendorRequestSearch.DateProcessedLT = DateTime.TryParse(objResults[1], out dtmTemp) ? dtmTemp : DateTime.MinValue;
                            break;
                        case "DateRequested":
                            objResults = DataTablesModel.sSearch_[intCounter].Split('~');//results returned from a daterange are delimited by the tilde char
                            objVendorRequestSearch.DateRequestedGT = DateTime.TryParse(objResults[0], out dtmTemp) ? dtmTemp : DateTime.MinValue;
                            objVendorRequestSearch.DateRequestedLT = DateTime.TryParse(objResults[1], out dtmTemp) ? dtmTemp : DateTime.MinValue;
                            break;
                        case "DateUpdated":
                            objResults = DataTablesModel.sSearch_[intCounter].Split('~');//results returned from a daterange are delimited by the tilde char
                            objVendorRequestSearch.DateUpdatedGT = DateTime.TryParse(objResults[0], out dtmTemp) ? dtmTemp : DateTime.MinValue;
                            objVendorRequestSearch.DateUpdatedLT = DateTime.TryParse(objResults[1], out dtmTemp) ? dtmTemp : DateTime.MinValue;
                            break;
                        case "SourceWarehouse":
                            objVendorRequestSearch.SourceWarehouses = DataTablesModel.sSearch_[intCounter].Split('|');//results returned from a checklist are delimited by the pipe char
                            break;
                        case "DestWarehouse":
                            objVendorRequestSearch.DestWarehouses = DataTablesModel.sSearch_[intCounter].Split('|');//results returned from a checklist are delimited by the pipe char
                            break;
                        case "OrderNo":
                            objVendorRequestSearch.OrderNo = DataTablesModel.sSearch_[intCounter];
                            break;
                        case "Item":
                            objVendorRequestSearch.Item = DataTablesModel.sSearch_[intCounter];
                            break;
                        case "LineNo":
                            break;
                        case "ReleaseNo":
                            break;
                        case "RequestCategoryID":
                            break;
                        case "RequestCategoryCode":
                            objVendorRequestSearch.RequestCategoryCodes = DataTablesModel.sSearch_[intCounter].Split('|');//results returned from a checklist are delimited by the pipe char
                            break;
                        case "Qty":
                            break;
                        case "QtyLoss":
                            break;
                        case "Approved":
                            break;
                        case "Creator":
                            break;
                        case "Updater":
                            break;
                    }
                }
            }

            /*I had considerable issues in capping the quantity of records returned by this method. My concern was that over the years I could have a scenario where thousands of Vendor Requests were in the database and so the code was written in a way 
             that all of those records would get loaded into memory and then they would be filtered.  In order to resolve this problem I had to initially populate the AllVendorRequests property of the InMemoryVendorRequestsRepository class with the filtered
             results. In order to do that I had to include an instance of the SL8VendorPortalDb datacontext class so that I could populate my filtered set of vendorrequests instead of first filling in the contents of the database into vendorrequests and then filtering
             * it there.  However, I ran into a problem when utilizing deferred execution(aka lazy loading) to first populate my list as filtered...  The below commented .Where() clauses in my query stopped working when querying against a database as opposed to 
             querying against a list in memory due to linq translating the query into SQL. I kept getting "String[]'. Only entity types, enumeration types or primitive types are supported in this context." and 
             "Cannot compare elements of type 'System.String[]'. Only primitive types, enumeration types and entity types are supported." errors because of this.*/

            /*The Below was created because the Entity Framework had a problem doing a filter of a list with a list because of the difficulty it had using deferred execution and the corresponding sql creation*/
            var strEmptyString = "EMPTY";
            var SourceWarehouseList = objVendorRequestSearch.SourceWarehouses == null ? new[] { strEmptyString } : objVendorRequestSearch.SourceWarehouses.ToArray<string>();
            var DestinationWarehouseList = objVendorRequestSearch.DestWarehouses == null ? new[] { strEmptyString } : objVendorRequestSearch.DestWarehouses.ToArray<string>();
            var RequestCategoryCodeList = objVendorRequestSearch.RequestCategoryCodes == null ? new[] { strEmptyString } : objVendorRequestSearch.RequestCategoryCodes.ToArray<string>();

            bool blnProcessed = false;
            if (objVendorRequestSearch.Processed != null && objVendorRequestSearch.Processed.ToUpper().Equals("BOTH"))
                blnProcessed = bool.TryParse(objVendorRequestSearch.Processed, out blnProcessed) ? blnProcessed : false;

            if (isDownloadReport)
                vendorrequests = VendorPortalDb.VendorRequests
                    .Where(c => objVendorRequestSearch.ID == null || SqlFunctions.StringConvert((double)c.ID).Contains(objVendorRequestSearch.ID))
                    .Where(c => objVendorRequestSearch.Item == null || c.Item.ToUpper().Contains(objVendorRequestSearch.Item.ToUpper()))
                    .Where(c => c.DateProcessed >= objVendorRequestSearch.DateProcessedGT || objVendorRequestSearch.DateProcessedGT == DateTime.MinValue)
                    .Where(c => c.DateProcessed <= objVendorRequestSearch.DateProcessedLT || objVendorRequestSearch.DateProcessedLT == DateTime.MinValue)
                    .Where(c => c.DateRequested >= objVendorRequestSearch.DateRequestedGT || objVendorRequestSearch.DateRequestedGT == DateTime.MinValue)
                    .Where(c => c.DateRequested <= objVendorRequestSearch.DateRequestedLT || objVendorRequestSearch.DateRequestedLT == DateTime.MinValue)
                    .Where(c => c.DateUpdated >= objVendorRequestSearch.DateUpdatedGT || objVendorRequestSearch.DateUpdatedGT == DateTime.MinValue)
                    .Where(c => c.DateUpdated <= objVendorRequestSearch.DateUpdatedLT || objVendorRequestSearch.DateUpdatedLT == DateTime.MinValue)
                    .Where(c => objVendorRequestSearch.OrderNo == null || ((c.OrderNo != null) && c.OrderNo.Contains(objVendorRequestSearch.OrderNo)))
                        //.ToList()//if I populated the query with a list right here (executed the query) then the below commented queries would function, however this didn't solve the problem I was facing where large collections of vendor requests would get loaded into memory prior to being filtered...
                        //The below commented queries only worked when doing it on a collection fully loaded into memory and stopped working when I implemented lazy loading to filter the initial collection.
                        //.Where(c => objVendorRequestSearch.Processed == null || objVendorRequestSearch.Processed.ToUpper().Equals("BOTH") || c.Processed.ToString().ToUpper().Equals(objVendorRequestSearch.Processed.ToUpper()))
                    .Where(c => objVendorRequestSearch.Processed == null || objVendorRequestSearch.Processed.ToUpper().Equals("BOTH") || c.Processed == blnProcessed)
                        //.Where(c => objVendorRequestSearch.SourceWarehouses == null || objVendorRequestSearch.SourceWarehouses.Contains(c.SourceWarehouse))
                    .Where(c => SourceWarehouseList.Contains(strEmptyString) || SourceWarehouseList.Contains(c.SourceWarehouse))
                        //.Where(c => objVendorRequestSearch.DestWarehouses == null || objVendorRequestSearch.DestWarehouses.Contains(c.DestWarehouse))
                     .Where(c => DestinationWarehouseList.Contains(strEmptyString) || DestinationWarehouseList.Contains(c.DestWarehouse))
                        //.Where(c => objVendorRequestSearch.RequestCategoryCodes == null || objVendorRequestSearch.RequestCategoryCodes.Contains(c.RequestCategoryCode))
                    .Where(c => RequestCategoryCodeList.Contains(strEmptyString) || RequestCategoryCodeList.Contains(c.RequestCategoryCode))
                    .ToList();
            else
                vendorrequests = VendorPortalDb.VendorRequests
                    .Where(c => objVendorRequestSearch.ID == null || SqlFunctions.StringConvert((double)c.ID).Contains(objVendorRequestSearch.ID))
                    .Where(c => objVendorRequestSearch.Item == null || c.Item.ToUpper().Contains(objVendorRequestSearch.Item.ToUpper()))
                    .Where(c => c.DateProcessed >= objVendorRequestSearch.DateProcessedGT || objVendorRequestSearch.DateProcessedGT == DateTime.MinValue)
                    .Where(c => c.DateProcessed <= objVendorRequestSearch.DateProcessedLT || objVendorRequestSearch.DateProcessedLT == DateTime.MinValue)
                    .Where(c => c.DateRequested >= objVendorRequestSearch.DateRequestedGT || objVendorRequestSearch.DateRequestedGT == DateTime.MinValue)
                    .Where(c => c.DateRequested <= objVendorRequestSearch.DateRequestedLT || objVendorRequestSearch.DateRequestedLT == DateTime.MinValue)
                    .Where(c => c.DateUpdated >= objVendorRequestSearch.DateUpdatedGT || objVendorRequestSearch.DateUpdatedGT == DateTime.MinValue)
                    .Where(c => c.DateUpdated <= objVendorRequestSearch.DateUpdatedLT || objVendorRequestSearch.DateUpdatedLT == DateTime.MinValue)
                    .Where(c => objVendorRequestSearch.OrderNo == null || ((c.OrderNo != null) && c.OrderNo.Contains(objVendorRequestSearch.OrderNo)))
                    //.ToList()//if I populated the query with a list right here (executed the query) then the below commented queries would function, however this didn't solve the problem I was facing where large collections of vendor requests would get loaded into memory prior to being filtered...
                    //The below commented queries only worked when doing it on a collection fully loaded into memory and stopped working when I implemented lazy loading to filter the initial collection.
                    //.Where(c => objVendorRequestSearch.Processed == null || objVendorRequestSearch.Processed.ToUpper().Equals("BOTH") || c.Processed.ToString().ToUpper().Equals(objVendorRequestSearch.Processed.ToUpper()))
                    .Where(c => objVendorRequestSearch.Processed == null || objVendorRequestSearch.Processed.ToUpper().Equals("BOTH") || c.Processed == blnProcessed)
                    //.Where(c => objVendorRequestSearch.SourceWarehouses == null || objVendorRequestSearch.SourceWarehouses.Contains(c.SourceWarehouse))
                    .Where(c => SourceWarehouseList.Contains(strEmptyString) || SourceWarehouseList.Contains(c.SourceWarehouse))
                    //.Where(c => objVendorRequestSearch.DestWarehouses == null || objVendorRequestSearch.DestWarehouses.Contains(c.DestWarehouse))
                     .Where(c => DestinationWarehouseList.Contains(strEmptyString) || DestinationWarehouseList.Contains(c.DestWarehouse))
                    //.Where(c => objVendorRequestSearch.RequestCategoryCodes == null || objVendorRequestSearch.RequestCategoryCodes.Contains(c.RequestCategoryCode))
                    .Where(c => RequestCategoryCodeList.Contains(strEmptyString) || RequestCategoryCodeList.Contains(c.RequestCategoryCode))
                    .Take(MaxRecordCount)
                    .ToList();
            
            searchRecordCount = vendorrequests.Count;

            IOrderedEnumerable<VendorRequest> sortedList = null;
            foreach (var sortedColumn in sortedColumns)
            {
                switch (sortedColumn.PropertyName)
                {
                    case "ID":
                        sortedList = sortedList == null ? vendorrequests.CustomSort(sortedColumn.Direction, i => i.ID)
                            : sortedList.CustomSort(sortedColumn.Direction, i => i.ID);
                        break;
                    case "Item":
                        sortedList = sortedList == null ? vendorrequests.CustomSort(sortedColumn.Direction, i => i.Item)
                            : sortedList.CustomSort(sortedColumn.Direction, i => i.Item);
                        break;
                    case "Processed":
                        sortedList = sortedList == null ? vendorrequests.CustomSort(sortedColumn.Direction, i => i.Processed)
                            : sortedList.CustomSort(sortedColumn.Direction, i => i.Processed);
                        break;
                    case "Notes":
                        sortedList = sortedList == null ? vendorrequests.CustomSort(sortedColumn.Direction, i => i.Notes)
                            : sortedList.CustomSort(sortedColumn.Direction, i => i.Notes);
                        break;
                    case "DateRequested":
                        sortedList = sortedList == null ? vendorrequests.CustomSort(sortedColumn.Direction, i => i.DateRequested)
                            : sortedList.CustomSort(sortedColumn.Direction, i => i.DateRequested);
                        break;
                    case "DateProcessed":
                        sortedList = sortedList == null ? vendorrequests.CustomSort(sortedColumn.Direction, i => i.DateProcessed)
                            : sortedList.CustomSort(sortedColumn.Direction, i => i.DateProcessed);
                        break;
                    case "SourceWarehouse":
                        sortedList = sortedList == null ? vendorrequests.CustomSort(sortedColumn.Direction, i => i.SourceWarehouse)
                            : sortedList.CustomSort(sortedColumn.Direction, i => i.SourceWarehouse);
                        break;
                    case "RequestCategoryCode":
                        sortedList = sortedList == null ? vendorrequests.CustomSort(sortedColumn.Direction, i => i.RequestCategoryCode)
                            : sortedList.CustomSort(sortedColumn.Direction, i => i.RequestCategoryCode);
                        break;
                    case "RequestCategoryID":
                        sortedList = sortedList == null ? vendorrequests.CustomSort(sortedColumn.Direction, i => i.RequestCategoryID)
                            : sortedList.CustomSort(sortedColumn.Direction, i => i.RequestCategoryID);
                        break;
                    case "DestWarehouse":
                        sortedList = sortedList == null ? vendorrequests.CustomSort(sortedColumn.Direction, i => i.DestWarehouse)
                            : sortedList.CustomSort(sortedColumn.Direction, i => i.DestWarehouse);
                        break;
                    case "OrderNo":
                        sortedList = sortedList == null ? vendorrequests.CustomSort(sortedColumn.Direction, i => i.OrderNo)
                            : sortedList.CustomSort(sortedColumn.Direction, i => i.OrderNo);
                        break;
                    case "LineNo":
                        sortedList = sortedList == null ? vendorrequests.CustomSort(sortedColumn.Direction, i => i.LineNo)
                            : sortedList.CustomSort(sortedColumn.Direction, i => i.LineNo);
                        break;
                    case "ReleaseNo":
                        sortedList = sortedList == null ? vendorrequests.CustomSort(sortedColumn.Direction, i => i.ReleaseNo)
                            : sortedList.CustomSort(sortedColumn.Direction, i => i.ReleaseNo);
                        break;
                    case "Qty":
                        sortedList = sortedList == null ? vendorrequests.CustomSort(sortedColumn.Direction, i => i.Qty)
                            : sortedList.CustomSort(sortedColumn.Direction, i => i.Qty);
                        break;
                    case "QtyLoss":
                        sortedList = sortedList == null ? vendorrequests.CustomSort(sortedColumn.Direction, i => i.QtyLoss)
                            : sortedList.CustomSort(sortedColumn.Direction, i => i.QtyLoss);
                        break;
                    case "Approved":
                        sortedList = sortedList == null ? vendorrequests.CustomSort(sortedColumn.Direction, i => i.Approved)
                            : sortedList.CustomSort(sortedColumn.Direction, i => i.Approved);
                        break;
                    case "Creator":
                        sortedList = sortedList == null ? vendorrequests.CustomSort(sortedColumn.Direction, i => i.Creator)
                            : sortedList.CustomSort(sortedColumn.Direction, i => i.Creator);
                        break;
                    case "Updater":
                        sortedList = sortedList == null ? vendorrequests.CustomSort(sortedColumn.Direction, i => i.Updater)
                            : sortedList.CustomSort(sortedColumn.Direction, i => i.Updater);
                        break;
                    default://This took care of the below scenario where the default sorted column was 0 which is my drill down image. I could have used 'case "0":' but just made a default case instead...
                        sortedList = sortedList == null ? vendorrequests.CustomSort(sortedColumn.Direction, i => i.ID)
                            : sortedList;
                        break;
                }
            }

            /*if (DataTablesModel.iDisplayLength == -1) //pagination is disabled in the javascript
                try { return sortedList.Skip(DataTablesModel.iDisplayStart).ToList(); }
                catch (ArgumentNullException) { return vendorrequests.Skip(DataTablesModel.iDisplayStart).ToList(); }
            else //pagination is enabled
                try { return sortedList.Skip(DataTablesModel.iDisplayStart).Take(DataTablesModel.iDisplayLength).ToList(); }
                catch (ArgumentNullException) { return vendorrequests.Skip(DataTablesModel.iDisplayStart).Take(DataTablesModel.iDisplayLength).ToList(); }*/
            if (isDownloadReport)
                return sortedList.ToList();
            else
                if (DataTablesModel.iDisplayLength == -1)
                    return sortedList.Skip(DataTablesModel.iDisplayStart).ToList();
                else
                    return sortedList.Skip(DataTablesModel.iDisplayStart).Take(DataTablesModel.iDisplayLength).ToList();
        }
        #region OldMethod
        //public static IList<VendorRequest> GetVendorRequests(out int totalRecordCount, out int searchRecordCount, JQueryDataTablesModel DataTablesModel)
        //{
        //    DateTime dtmTemp;
        //    string[] objResults;
        //    VendorRequestSearch objVendorRequestSearch;
        //    ReadOnlyCollection<SortedColumn> sortedColumns = DataTablesModel.GetSortedColumns();
        //    var vendorrequests = AllVendorRequests;

        //    totalRecordCount = vendorrequests.Count;


        //    objVendorRequestSearch = new VendorRequestSearch();
        //    for (int intCounter = 0; intCounter < DataTablesModel.iColumns; intCounter++ )
        //    {

        //        if (DataTablesModel.bSearchable_[intCounter] == true && !string.IsNullOrEmpty(DataTablesModel.sSearch_[intCounter]))
        //        {
        //            /*For some reason when I implemented resizable movable columns and would then move the columns in the application, the application would send tilde's in the 'checkbox' column types sSearch field which was wierd
        //             since the checkbox column types are delimited by the pipe | character and the 'range' column types are delimited by the tilde...  The resolution that I came up with was to check if the only value passed in sSearch
        //             was a tilde and if it was then skip the loop so that the respective VendorRequestSearch field was left null.*/
        //            if (DataTablesModel.sSearch_[intCounter].Equals("~"))
        //                continue;

        //            /*Notice that i had to use mDataProp2_ due to datatables multi-column filtering not placing sSearch into proper array position when columns are reordered; See VendorRequestsController.cs Search method for details...*/
        //            switch (DataTablesModel.mDataProp2_[intCounter])
        //            {
        //                case "ID":
        //                    objVendorRequestSearch.ID = DataTablesModel.sSearch_[intCounter];
        //                    break;
        //                case "Processed":
        //                    objResults = DataTablesModel.sSearch_[intCounter].Split('|');//results returned from a checklist are delimited by the pipe char
        //                    if (objResults.Length > 1)//there are two results returned so both true and false have been selected
        //                        objVendorRequestSearch.Processed = "Both";
        //                    else
        //                        objVendorRequestSearch.Processed = objResults[0];  
        //                    break;
        //                case "Notes":
        //                    break;
        //                case "DateProcessed":
        //                    objResults = DataTablesModel.sSearch_[intCounter].Split('~');//results returned from a daterange are delimited by the tilde char
        //                    objVendorRequestSearch.DateProcessedGT = DateTime.TryParse(objResults[0], out dtmTemp) ? dtmTemp : DateTime.MinValue;
        //                    objVendorRequestSearch.DateProcessedLT = DateTime.TryParse(objResults[1], out dtmTemp) ? dtmTemp : DateTime.MinValue;
        //                    break;
        //                case "DateRequested":
        //                    objResults = DataTablesModel.sSearch_[intCounter].Split('~');//results returned from a daterange are delimited by the tilde char
        //                    objVendorRequestSearch.DateRequestedGT = DateTime.TryParse(objResults[0], out dtmTemp) ? dtmTemp : DateTime.MinValue;
        //                    objVendorRequestSearch.DateRequestedLT = DateTime.TryParse(objResults[1], out dtmTemp) ? dtmTemp : DateTime.MinValue;
        //                    break;
        //                case "DateUpdated":
        //                    objResults = DataTablesModel.sSearch_[intCounter].Split('~');//results returned from a daterange are delimited by the tilde char
        //                    objVendorRequestSearch.DateUpdatedGT = DateTime.TryParse(objResults[0], out dtmTemp) ? dtmTemp : DateTime.MinValue;
        //                    objVendorRequestSearch.DateUpdatedLT = DateTime.TryParse(objResults[1], out dtmTemp) ? dtmTemp : DateTime.MinValue;
        //                    break;
        //                case "SourceWarehouse":
        //                    objVendorRequestSearch.SourceWarehouses = DataTablesModel.sSearch_[intCounter].Split('|');//results returned from a checklist are delimited by the pipe char
        //                    break;
        //                case "DestWarehouse":
        //                    objVendorRequestSearch.DestWarehouses = DataTablesModel.sSearch_[intCounter].Split('|');//results returned from a checklist are delimited by the pipe char
        //                    break;
        //                case "OrderNo":
        //                    objVendorRequestSearch.OrderNo = DataTablesModel.sSearch_[intCounter];
        //                    break;
        //                case "Item":
        //                    objVendorRequestSearch.Item = DataTablesModel.sSearch_[intCounter];
        //                    break;
        //                case "LineNo":
        //                    break;
        //                case "ReleaseNo":
        //                    break;
        //                case "RequestCategoryID":
        //                    break;
        //                case "RequestCategoryCode":
        //                    objVendorRequestSearch.RequestCategoryCodes = DataTablesModel.sSearch_[intCounter].Split('|');//results returned from a checklist are delimited by the pipe char
        //                    break;
        //                case "Qty":
        //                    break;
        //                case "QtyLoss":
        //                    break;
        //                case "Approved":
        //                    break;
        //                case "Creator":
        //                    break;
        //                case "Updater":
        //                    break;
        //            }
        //        }
        //    }
            
        //    vendorrequests = vendorrequests
        //        .Where(c => string.IsNullOrEmpty(objVendorRequestSearch.ID) || c.ID.ToString().Contains(objVendorRequestSearch.ID))
        //        .Where(c => string.IsNullOrEmpty(objVendorRequestSearch.Item) || c.Item.ToUpper().Contains(objVendorRequestSearch.Item.ToUpper()))
        //        .Where(c => c.DateProcessed >= objVendorRequestSearch.DateProcessedGT || objVendorRequestSearch.DateProcessedGT == DateTime.MinValue)
        //        .Where(c => c.DateProcessed <= objVendorRequestSearch.DateProcessedLT || objVendorRequestSearch.DateProcessedLT == DateTime.MinValue)
        //        .Where(c => c.DateRequested >= objVendorRequestSearch.DateRequestedGT || objVendorRequestSearch.DateRequestedGT == DateTime.MinValue)
        //        .Where(c => c.DateRequested <= objVendorRequestSearch.DateRequestedLT || objVendorRequestSearch.DateRequestedLT == DateTime.MinValue)
        //        .Where(c => c.DateUpdated >= objVendorRequestSearch.DateUpdatedGT || objVendorRequestSearch.DateUpdatedGT == DateTime.MinValue)
        //        .Where(c => c.DateUpdated <= objVendorRequestSearch.DateUpdatedLT || objVendorRequestSearch.DateUpdatedLT == DateTime.MinValue)
        //        .Where(c => string.IsNullOrEmpty(objVendorRequestSearch.Processed) || objVendorRequestSearch.Processed.ToUpper().Equals("BOTH") || c.Processed.ToString().ToUpper().Equals(objVendorRequestSearch.Processed.ToUpper()))
        //        .Where(c => objVendorRequestSearch.SourceWarehouses == null || objVendorRequestSearch.SourceWarehouses.Contains(c.SourceWarehouse))
        //        .Where(c => objVendorRequestSearch.DestWarehouses == null || objVendorRequestSearch.DestWarehouses.Contains(c.DestWarehouse))
        //        .Where(c => string.IsNullOrEmpty(objVendorRequestSearch.OrderNo) || (!string.IsNullOrEmpty(c.OrderNo) && c.OrderNo.ToString().Contains(objVendorRequestSearch.OrderNo)))
        //        .Where(c => objVendorRequestSearch.RequestCategoryCodes == null || objVendorRequestSearch.RequestCategoryCodes.Contains(c.RequestCategoryCode))
        //        .ToList();


        //    searchRecordCount = vendorrequests.Count;

        //    IOrderedEnumerable<VendorRequest> sortedList = null;
        //    foreach (var sortedColumn in sortedColumns)
        //    {
        //        switch (sortedColumn.PropertyName)
        //        {
        //            case "ID":
        //                sortedList = sortedList == null ? vendorrequests.CustomSort(sortedColumn.Direction, i => i.ID)
        //                    : sortedList.CustomSort(sortedColumn.Direction, i => i.ID);
        //                break;
        //            case "Item":
        //                sortedList = sortedList == null ? vendorrequests.CustomSort(sortedColumn.Direction, i => i.Item)
        //                    : sortedList.CustomSort(sortedColumn.Direction, i => i.Item);
        //                break;
        //            case "Processed":
        //                sortedList = sortedList == null ? vendorrequests.CustomSort(sortedColumn.Direction, i => i.Processed)
        //                    : sortedList.CustomSort(sortedColumn.Direction, i => i.Processed);
        //                break;
        //            case "Notes":
        //                sortedList = sortedList == null ? vendorrequests.CustomSort(sortedColumn.Direction, i => i.Notes)
        //                    : sortedList.CustomSort(sortedColumn.Direction, i => i.Notes);
        //                break;
        //            case "DateRequested":
        //                sortedList = sortedList == null ? vendorrequests.CustomSort(sortedColumn.Direction, i => i.DateRequested)
        //                    : sortedList.CustomSort(sortedColumn.Direction, i => i.DateRequested);
        //                break;
        //            case "DateProcessed":
        //                sortedList = sortedList == null ? vendorrequests.CustomSort(sortedColumn.Direction, i => i.DateProcessed)
        //                    : sortedList.CustomSort(sortedColumn.Direction, i => i.DateProcessed);
        //                break;
        //            case "SourceWarehouse":
        //                sortedList = sortedList == null ? vendorrequests.CustomSort(sortedColumn.Direction, i => i.SourceWarehouse)
        //                    : sortedList.CustomSort(sortedColumn.Direction, i => i.SourceWarehouse);
        //                break;
        //            case "RequestCategoryCode":
        //                sortedList = sortedList == null ? vendorrequests.CustomSort(sortedColumn.Direction, i => i.RequestCategoryCode)
        //                    : sortedList.CustomSort(sortedColumn.Direction, i => i.RequestCategoryCode);
        //                break;
        //            case "RequestCategoryID":
        //                sortedList = sortedList == null ? vendorrequests.CustomSort(sortedColumn.Direction, i => i.RequestCategoryID)
        //                    : sortedList.CustomSort(sortedColumn.Direction, i => i.RequestCategoryID);
        //                break;
        //            case "DestWarehouse":
        //                sortedList = sortedList == null ? vendorrequests.CustomSort(sortedColumn.Direction, i => i.DestWarehouse)
        //                    : sortedList.CustomSort(sortedColumn.Direction, i => i.DestWarehouse);
        //                break;
        //            case "OrderNo":
        //                sortedList = sortedList == null ? vendorrequests.CustomSort(sortedColumn.Direction, i => i.OrderNo)
        //                    : sortedList.CustomSort(sortedColumn.Direction, i => i.OrderNo);
        //                break;
        //            case "LineNo":
        //                sortedList = sortedList == null ? vendorrequests.CustomSort(sortedColumn.Direction, i => i.LineNo)
        //                    : sortedList.CustomSort(sortedColumn.Direction, i => i.LineNo);
        //                break;
        //            case "ReleaseNo":
        //                sortedList = sortedList == null ? vendorrequests.CustomSort(sortedColumn.Direction, i => i.ReleaseNo)
        //                    : sortedList.CustomSort(sortedColumn.Direction, i => i.ReleaseNo);
        //                break;
        //            case "Qty":
        //                sortedList = sortedList == null ? vendorrequests.CustomSort(sortedColumn.Direction, i => i.Qty)
        //                    : sortedList.CustomSort(sortedColumn.Direction, i => i.Qty);
        //                break;
        //            case "QtyLoss":
        //                sortedList = sortedList == null ? vendorrequests.CustomSort(sortedColumn.Direction, i => i.QtyLoss)
        //                    : sortedList.CustomSort(sortedColumn.Direction, i => i.QtyLoss);
        //                break;
        //            case "Approved":
        //                sortedList = sortedList == null ? vendorrequests.CustomSort(sortedColumn.Direction, i => i.Approved)
        //                    : sortedList.CustomSort(sortedColumn.Direction, i => i.Approved);
        //                break;
        //            case "Creator":
        //                sortedList = sortedList == null ? vendorrequests.CustomSort(sortedColumn.Direction, i => i.Creator)
        //                    : sortedList.CustomSort(sortedColumn.Direction, i => i.Creator);
        //                break;
        //            case "Updater":
        //                sortedList = sortedList == null ? vendorrequests.CustomSort(sortedColumn.Direction, i => i.Updater)
        //                    : sortedList.CustomSort(sortedColumn.Direction, i => i.Updater);
        //                break;
        //        }
        //    }

        //    if (DataTablesModel.iDisplayLength == -1) //pagination is disabled in the javascript
        //        try { return sortedList.Skip(DataTablesModel.iDisplayStart).ToList(); }
        //        catch (ArgumentNullException) { return vendorrequests.Skip(DataTablesModel.iDisplayStart).ToList(); }
        //    else //pagination is enabled
        //        try { return sortedList.Skip(DataTablesModel.iDisplayStart).Take(DataTablesModel.iDisplayLength).ToList(); }
        //        catch (ArgumentNullException) { return vendorrequests.Skip(DataTablesModel.iDisplayStart).Take(DataTablesModel.iDisplayLength).ToList(); }
        //}
        #endregion

        private static bool CheckTilde(string strSearch)
        {
            if (!string.IsNullOrEmpty(strSearch))
                if (strSearch.Split('|')[0].Equals("~"))
                    return true;
            return false;
        }

        public static IList<VendorRequest> GetVendorRequests(int startIndex, int pageSize, ReadOnlyCollection<SortedColumn> sortedColumns, out int totalRecordCount,
            out int searchRecordCount, string searchString)
        {
            var vendorrequests = AllVendorRequests;

            totalRecordCount = vendorrequests.Count;

            if (!string.IsNullOrWhiteSpace(searchString))
            {
                vendorrequests = vendorrequests.Where(c => c.OrderNo.ToLower().Contains(searchString.ToLower()))
                    .ToList();
            }

            searchRecordCount = vendorrequests.Count;

            IOrderedEnumerable<VendorRequest> sortedList = null;
            foreach (var sortedColumn in sortedColumns)
            {
                switch (sortedColumn.PropertyName)
                {
                    case "ID":
                        sortedList = sortedList == null ? vendorrequests.CustomSort(sortedColumn.Direction, i => i.ID)
                            : sortedList.CustomSort(sortedColumn.Direction, i => i.ID);
                        break;
                    case "Processed":
                        sortedList = sortedList == null ? vendorrequests.CustomSort(sortedColumn.Direction, i => i.Processed)
                            : sortedList.CustomSort(sortedColumn.Direction, i => i.Processed);
                        break;
                    case "Notes":
                        sortedList = sortedList == null ? vendorrequests.CustomSort(sortedColumn.Direction, i => i.Notes)
                            : sortedList.CustomSort(sortedColumn.Direction, i => i.Notes);
                        break;
                    case "DateRequested":
                        sortedList = sortedList == null ? vendorrequests.CustomSort(sortedColumn.Direction, i => i.DateRequested)
                            : sortedList.CustomSort(sortedColumn.Direction, i => i.DateRequested);
                        break;
                    case "DateProcessed":
                        sortedList = sortedList == null ? vendorrequests.CustomSort(sortedColumn.Direction, i => i.DateProcessed)
                            : sortedList.CustomSort(sortedColumn.Direction, i => i.DateProcessed);
                        break;
                    case "DateUpdated":
                        sortedList = sortedList == null ? vendorrequests.CustomSort(sortedColumn.Direction, i => i.DateUpdated)
                            : sortedList.CustomSort(sortedColumn.Direction, i => i.DateUpdated);
                        break;
                    case "SourceWarehouse":
                        sortedList = sortedList == null ? vendorrequests.CustomSort(sortedColumn.Direction, i => i.SourceWarehouse)
                            : sortedList.CustomSort(sortedColumn.Direction, i => i.SourceWarehouse);
                        break;
                    case "RequestType":
                        sortedList = sortedList == null ? vendorrequests.CustomSort(sortedColumn.Direction, i => i.RequestCategoryCode)
                            : sortedList.CustomSort(sortedColumn.Direction, i => i.RequestCategoryCode);
                        break;
                    case "RequestCategoryID":
                        sortedList = sortedList == null ? vendorrequests.CustomSort(sortedColumn.Direction, i => i.RequestCategoryID)
                            : sortedList.CustomSort(sortedColumn.Direction, i => i.RequestCategoryID);
                        break;
                    case "RequestCategoryCode":
                        sortedList = sortedList == null ? vendorrequests.CustomSort(sortedColumn.Direction, i => i.RequestCategoryCode)
                            : sortedList.CustomSort(sortedColumn.Direction, i => i.RequestCategoryCode);
                        break;
                    case "DestWarehouse":
                        sortedList = sortedList == null ? vendorrequests.CustomSort(sortedColumn.Direction, i => i.DestWarehouse)
                            : sortedList.CustomSort(sortedColumn.Direction, i => i.DestWarehouse);
                        break;
                    case "OrderNo":
                        sortedList = sortedList == null ? vendorrequests.CustomSort(sortedColumn.Direction, i => i.OrderNo)
                            : sortedList.CustomSort(sortedColumn.Direction, i => i.OrderNo);
                        break;
                    case "LineNo)":
                        sortedList = sortedList == null ? vendorrequests.CustomSort(sortedColumn.Direction, i => i.LineNo)
                            : sortedList.CustomSort(sortedColumn.Direction, i => i.LineNo);
                        break;
                    case "ReleaseNo)":
                        sortedList = sortedList == null ? vendorrequests.CustomSort(sortedColumn.Direction, i => i.ReleaseNo)
                            : sortedList.CustomSort(sortedColumn.Direction, i => i.ReleaseNo);
                        break;
                    case "Qty":
                        sortedList = sortedList == null ? vendorrequests.CustomSort(sortedColumn.Direction, i => i.Qty)
                            : sortedList.CustomSort(sortedColumn.Direction, i => i.Qty);
                        break;
                    case "QtyLoss":
                        sortedList = sortedList == null ? vendorrequests.CustomSort(sortedColumn.Direction, i => i.QtyLoss)
                            : sortedList.CustomSort(sortedColumn.Direction, i => i.QtyLoss);
                        break;
                    case "Approved":
                        sortedList = sortedList == null ? vendorrequests.CustomSort(sortedColumn.Direction, i => i.Approved)
                            : sortedList.CustomSort(sortedColumn.Direction, i => i.Approved);
                        break;
                }
            }

            if (pageSize == -1) //pagination is disabled in the javascript
                try { return sortedList.Skip(startIndex).ToList(); }
                catch (ArgumentNullException) { return vendorrequests.Skip(startIndex).ToList(); }
            else //pagination is enabled
                try { return sortedList.Skip(startIndex).Take(pageSize).ToList(); }
                catch (ArgumentNullException) { return vendorrequests.Skip(startIndex).Take(pageSize).ToList(); }
        }
    }
}