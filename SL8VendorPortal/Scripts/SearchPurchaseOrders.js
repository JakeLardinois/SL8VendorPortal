var anOpen = [];
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
            { "mDataProp": "po_num" },
            { "mDataProp": "vend_num" },
            { "mDataProp": "order_date" },
            { "mDataProp": "ship_code" },
            { "mDataProp": "terms_code" },
            { "mDataProp": "fob" },
            { "mDataProp": "type" },
            { "mDataProp": "eff_date" },
            { "mDataProp": "whse" },
            { "mDataProp": "RecordDate" },
            { "mDataProp": "buyer" },
            { "mDataProp": "CreatedBy" },
            { "mDataProp": "UpdatedBy" },
            { "mDataProp": "CreateDate" }],
            "fnRowCallback": function (nRow, aData, iDisplayIndex, iDisplayIndexFull) {
                var objOrderdate = new Date(parseInt(aData.order_date.replace("/Date(", "").replace(")/", ""), 10));
                $('td:eq(3)', nRow).html(objOrderdate.getMonth() + 1 + "/" + objOrderdate.getDate() + "/" + objOrderdate.getFullYear());

                if (aData.eff_date != null && aData.eff_date !== undefined) {
                    var objEffdate = new Date(parseInt(aData.eff_date.replace("/Date(", "").replace(")/", ""), 10));
                    $('td:eq(8)', nRow).html(objEffdate.getMonth() + 1 + "/" + objEffdate.getDate() + "/" + objEffdate.getFullYear());
                }

                if (aData.RecordDate != null && aData.RecordDate !== undefined) {
                    var objRecordDate = new Date(parseInt(aData.RecordDate.replace("/Date(", "").replace(")/", ""), 10));
                    $('td:eq(10)', nRow).html(objRecordDate.getMonth() + 1 + "/" + objRecordDate.getDate() + "/" + objRecordDate.getFullYear());
                }

                if (aData.CreateDate != null && aData.CreateDate !== undefined) {
                    var objCreateDate = new Date(parseInt(aData.CreateDate.replace("/Date(", "").replace(")/", ""), 10));
                    $('td:eq(14)', nRow).html(objCreateDate.getMonth() + 1 + "/" + objCreateDate.getDate() + "/" + objCreateDate.getFullYear());
                }
            }
    });

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

        sOrderNo = oTable.fnGetData(rowIndex).po_num; //get the order number of the current row

        sLineURL = document.URL.replace(/Search$/, ""); //uses a RegEx to remove the string 'Search' only if it is at the end of the URL 
        sLineURL = sLineURL + 'SearchPOLinesByOrder';

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
                        { "mDataProp": "po_num", "sWidth": 30, fnRender: makeViewNotesBtn, "bSortable": false, "bSearchable": false },
                        { "mDataProp": "po_line" },
                        { "mDataProp": "po_release" },
                        { "mDataProp": "qty_ordered" },
                        { "mDataProp": "qty_received" },
                        { "mDataProp": "qty_rejected" },
                        { "mDataProp": "due_date" },
                        { "mDataProp": "rcvd_date" },
                        { "mDataProp": "CreateDate"}],
                "fnRowCallback": function (nRow, aData, iDisplayIndex, iDisplayIndexFull) {

                    if (aData.due_date != null && aData.due_date !== undefined) {
                        var objduedate = new Date(parseInt(aData.due_date.replace("/Date(", "").replace(")/", ""), 10));
                        $('td:eq(6)', nRow).html(objduedate.getMonth() + 1 + "/" + objduedate.getDate() + "/" + objduedate.getFullYear());
                    }

                    if (aData.ship_date != null && aData.ship_date !== undefined) {
                        var objrcvddate = new Date(parseInt(aData.rcvd_date.replace("/Date(", "").replace(")/", ""), 10));
                        $('td:eq(7)', nRow).html(objrcvddate.getMonth() + 1 + "/" + objrcvddate.getDate() + "/" + objrcvddate.getFullYear());
                    }

                    if (aData.CreateDate != null && aData.CreateDate !== undefined) {
                        var objCreateDate = new Date(parseInt(aData.CreateDate.replace("/Date(", "").replace(")/", ""), 10));
                        $('td:eq(8)', nRow).html(objCreateDate.getMonth() + 1 + "/" + objCreateDate.getDate() + "/" + objCreateDate.getFullYear());
                    }
                }
            });
        }
        else {
            $('img', this).attr('src', sOpenImageUrl);
            $('div.innerDetails', $(nTr).next()[0]).slideUp(function () {
                oTable.fnClose(nTr);
                anOpen.splice(i, 1);
            });
        }
    });
});

function makeViewNotesBtn(oObj) {
    var sOrderNo = oObj.aData.po_num;
    var sLineNo = oObj.aData.po_line;
    var sReleaseNo = oObj.aData.po_release;
    return "<a href='" + sLineURL + '?&OrderNo=' + sOrderNo + '&LineNo=' + sLineNo + '&ReleaseNo=' + sReleaseNo + "' class='ViewNotes' title='View Notes'><img src='" + sOpenImageUrl + "' height='12' width='12'></a>";

}