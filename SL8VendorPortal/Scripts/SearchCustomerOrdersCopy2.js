﻿var anOpen = [];
var oTable;


$(document).ready(function () {
    oTable = $('#objItems').dataTable({
        "bProcessing": true,
        "bServerSide": true,
        "sAjaxSource": document.URL,
        "sServerMethod": "POST",
        "aoColumns": [
            {
                "mDataProp": null, //Note that I had a problem with this column being first because when the datatable loads, it automatically sorts based on the first column; since this column had a null value
                "bSortable": false, //it would pass that null value to the data call. I actually fixed this by modifying the code in InMemoryRepositories.cs so that if there was an error, it just returns the list
                "sClass": "control center",
                "sDefaultContent": '<img src="' + sOpenImageUrl + '">'
            },
            { "mDataProp": "co_num" },
            { "mDataProp": "type" },
            { "mDataProp": "cust_num" },
            { "mDataProp": "cust_po" },
            { "mDataProp": "order_date" },
            { "mDataProp": "taken_by" },
            { "mDataProp": "ship_code" },
            { "mDataProp": "weight" },
            { "mDataProp": "qty_packages" },
            { "mDataProp": "slsman" },
            { "mDataProp": "eff_date" },
            { "mDataProp": "exp_date" },
            { "mDataProp": "whse" },
            { "mDataProp": "ship_partial" },
            { "mDataProp": "ship_early" },
            { "mDataProp": "projected_date" },
            { "mDataProp": "RecordDate" },
            { "mDataProp": "CreatedBy" },
            { "mDataProp": "UpdatedBy" },
            { "mDataProp": "CreateDate"}],
        "fnRowCallback": function (nRow, aData, iDisplayIndex, iDisplayIndexFull) {
            var objOrderdate = new Date(parseInt(aData.order_date.replace("/Date(", "").replace(")/", ""), 10));
            $('td:eq(5)', nRow).html(objOrderdate.getMonth() + 1 + "/" + objOrderdate.getDate() + "/" + objOrderdate.getFullYear());

            if (aData.eff_date != null && aData.eff_date !== undefined) {
                var objEffdate = new Date(parseInt(aData.eff_date.replace("/Date(", "").replace(")/", ""), 10));
                $('td:eq(11)', nRow).html(objEffdate.getMonth() + 1 + "/" + objEffdate.getDate() + "/" + objEffdate.getFullYear());
            }

            if (aData.exp_date != null && aData.exp_date !== undefined) {
                var objExpdate = new Date(parseInt(aData.exp_date.replace("/Date(", "").replace(")/", ""), 10));
                $('td:eq(12)', nRow).html(objExpdate.getMonth() + 1 + "/" + objExpdate.getDate() + "/" + objExpdate.getFullYear());
            }

            if (aData.projected_date != null && aData.projected_date !== undefined) {
                var objProjecteddate = new Date(parseInt(aData.projected_date.replace("/Date(", "").replace(")/", ""), 10));
                $('td:eq(16)', nRow).html(objProjecteddate.getMonth() + 1 + "/" + objProjecteddate.getDate() + "/" + objProjecteddate.getFullYear());
            }

            if (aData.RecordDate != null && aData.RecordDate !== undefined) {
                var objRecordDate = new Date(parseInt(aData.RecordDate.replace("/Date(", "").replace(")/", ""), 10));
                $('td:eq(17)', nRow).html(objRecordDate.getMonth() + 1 + "/" + objRecordDate.getDate() + "/" + objRecordDate.getFullYear());
            }

            if (aData.CreateDate != null && aData.CreateDate !== undefined) {
                var objCreateDate = new Date(parseInt(aData.CreateDate.replace("/Date(", "").replace(")/", ""), 10));
                $('td:eq(20)', nRow).html(objCreateDate.getMonth() + 1 + "/" + objCreateDate.getDate() + "/" + objCreateDate.getFullYear());
            }
        }
    });


    //        $('#objItems tbody').on('click', 'td.control', function () { //specific td item is clicked
    //        $('#objItems tbody').on('click', 'td', function () {          //any td item clicked
    //        $('#objItems tbody').on('click', 'td.control', function () {  //anytime  row is clicked
    $('#objItems tbody').on('click', 'td.control', function () {
        var nTr;
        var i;
        var rowIndex;
        var nDetailsRow;
        var sLineTableName;
        var sLineURL;
        var sOrderNo;


        nTr = this.parentNode;
        i = $.inArray(nTr, anOpen);

        rowIndex = oTable.fnGetPosition(nTr); //get the index of the current row
        sLineTableName = 'dtLineTable' + rowIndex;

        //alert(oTable.fnGetData(oTable.$('tr.row_selected')[0])[rowIndex].co_num);// works!!
        sOrderNo = oTable.fnGetData(rowIndex).co_num; //get the order number of the current row

        sLineURL = document.URL.replace(/Search$/, ""); //uses a RegEx to remove the string 'Search' only if it is at the end of the URL 
        sLineURL = sLineURL + 'SearchCOLinesByOrder';

        //I used the below code to see if the row was opening or closing based on the image that was loaded. I ended up not needing this because of the i variable below...
        /*if ($('img', this).attr('src') === sOpenImageUrl) { //check to see if the row is open
        //alert('Row is open');
        }
        else {
        //alert('Row is closed');
        }*/

        if (i === -1) { //the datatable is opening the row...
            $('img', this).attr('src', sCloseImageUrl);
            nDetailsRow = oTable.fnOpen(nTr, fnFormatDetails(oTable, nTr, sLineTableName), 'details');
            $('div.innerDetails', nDetailsRow).slideDown();
            anOpen.push(nTr);


            var tInnerTable = $('#' + sLineTableName).dataTable({ //when referencing the table for it's api, it always wants to be prefaced with #
                "bProcessing": true,
                "bFilter": false,   //hides the search box
                "bPaginate": false, //disables paging functionality
                "bServerSide": true,
                "sAjaxSource": sLineURL + '?&OrderNo=' + sOrderNo, //pass the order number to the orderline url as a querystring; note query string variables are delimited by &
                "sDom": '<"top">rt<"bottom"flp><"clear">', //hides the filter box and the 'showing recordno of records' message
                "sServerMethod": "POST", "aoColumns": [
                        { "mDataProp": "co_num", "sWidth": 30, fnRender: makeViewNotesBtn, "bSortable": false, "bSearchable": false },
                        { "mDataProp": "co_line" },
                        { "mDataProp": "co_release" },
                        { "mDataProp": "qty_ordered" },
                        { "mDataProp": "qty_shipped" },
                        { "mDataProp": "due_date" },
                        { "mDataProp": "ship_date" },
                        { "mDataProp": "projected_date" },
                        { "mDataProp": "CreateDate"}],
                /*"fnDrawCallback": function () {    // add a class for fancybox call to delete icons' href (defined in make_delete_link()
                $(".ViewNotes").fancybox({
                'titlePosition': 'top',
                'overlayColor': '#D03B40',
                'overlayOpacity': 0.4,
                'transitionOut': 'elastic',
                'transitionIn': 'elastic',
                'height': '65%',
                'width': '85%',
                'scrolling': 'auto',
                'speedIn': 500 })
                },*/
                "fnRowCallback": function (nRow, aData, iDisplayIndex, iDisplayIndexFull) {

                    if (aData.due_date != null && aData.due_date !== undefined) {
                        var objduedate = new Date(parseInt(aData.due_date.replace("/Date(", "").replace(")/", ""), 10));
                        $('td:eq(4)', nRow).html(objduedate.getMonth() + 1 + "/" + objduedate.getDate() + "/" + objduedate.getFullYear());
                    }

                    if (aData.ship_date != null && aData.ship_date !== undefined) {
                        var objshipdate = new Date(parseInt(aData.ship_date.replace("/Date(", "").replace(")/", ""), 10));
                        $('td:eq(5)', nRow).html(objshipdate.getMonth() + 1 + "/" + objshipdate.getDate() + "/" + objshipdate.getFullYear());
                    }

                    if (aData.projected_date != null && aData.projected_date !== undefined) {
                        var objProjecteddate = new Date(parseInt(aData.projected_date.replace("/Date(", "").replace(")/", ""), 10));
                        $('td:eq(6)', nRow).html(objProjecteddate.getMonth() + 1 + "/" + objProjecteddate.getDate() + "/" + objProjecteddate.getFullYear());
                    }

                    if (aData.CreateDate != null && aData.CreateDate !== undefined) {
                        var objCreateDate = new Date(parseInt(aData.CreateDate.replace("/Date(", "").replace(")/", ""), 10));
                        $('td:eq(7)', nRow).html(objCreateDate.getMonth() + 1 + "/" + objCreateDate.getDate() + "/" + objCreateDate.getFullYear());
                    }
                }
            });


        }
        else { //the datatable is closing the row...
            //$(sLineTableName).remove(); //I didn't need to remove the datatable once I was dynamically naming them based on row number

            $('img', this).attr('src', sOpenImageUrl);
            $('div.innerDetails', $(nTr).next()[0]).slideUp(function () {
                oTable.fnClose(nTr);
                anOpen.splice(i, 1);
            });
        }
    });

    /*// make a link to delete an id's row in the database.  oObj is defined as object passed to <a href="/ref#fnRender">fnRender</a> // you could just make this an anonymous function in-line with the aoColumns definition
    function makeViewNotesBtn(oObj) {
        // I'm relying on id being in column 0, but I could just as well used 
        //  oObj.aData[oObj.iDataColumn]; // since this column also has the ID value
        var id = oObj.aData[0];

//        var sLineURL;
//        var sOrderNo = oObj.aData.co_num;


        sLineURL = document.URL.replace(/Search$/, ""); //uses a RegEx to remove the string 'Search' only if it is at the end of the URL 
        sLineURL = sLineURL + 'GetOrderNotes';

        //return "<a href='db_delete.php?table=publications&id=" + id + "&nojq' class='delete' title='delete " + id + " from database'><img src='graphics/delete.png' height='12' width='12'></a>";
        return "<a href='" + sLineURL + '?&OrderNo=' + id + "' class='ViewNotes' title='View Notes'><img src='" + sOpenImageUrl + "' height='12' width='12'></a>";

    }*/



    //        function fnFormatDetails(oTable, nTr) {
    //            var oData = oTable.fnGetData(nTr);
    //            var sOut =
    //                '<div class="innerDetails">' +
    //                    '<table cellpadding="5" cellspacing="0" border="0" style="padding-left:50px;">' +
    //                    '<tr><td>Rendering engine:</td><td>' + oData.engine + '</td></tr>' +
    //                    '<tr><td>Browser:</td><td>' + oData.browser + '</td></tr>' +
    //                    '<tr><td>Platform:</td><td>' + oData.platform + '</td></tr>' +
    //                    '<tr><td>Version:</td><td>' + oData.version + '</td></tr>' +
    //                    '<tr><td>Grade:</td><td>' + oData.grade + '</td></tr>' +
    //                    '</table>' +
    //                '</div>';
    //            return sOut;
    //        }
});

//    $(document).ready(function () {
//        /* Init DataTables */
//        $('#objItems').dataTable();

//        /* Add events */
//        $('#objItems tbody').on('click', 'tr', function () {
//            var sTitle;
//            var nTds = $('td', this);
//            var sBrowser = $(nTds[1]).text();
//            var sGrade = $(nTds[4]).text();

//            if (sGrade == "A")
//                sTitle = sBrowser + ' will provide a first class (A) level of CSS support.';
//            else if (sGrade == "C")
//                sTitle = sBrowser + ' will provide a core (C) level of CSS support.';
//            else if (sGrade == "X")
//                sTitle = sBrowser + ' does not provide CSS support or has a broken implementation. Block CSS.';
//            else
//                sTitle = sBrowser + ' will provide an undefined level of CSS support.';

//            alert(sTitle)
//        });
//    });