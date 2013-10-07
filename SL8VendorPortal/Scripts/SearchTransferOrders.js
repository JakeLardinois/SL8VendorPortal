var anOpen = [];
var oTable;


$(document).ready(function () {
    oTable = $('#objItems').dataTable({
            "bProcessing": true,
            "bServerSide": true,
            "sAjaxSource": document.URL,
            "sServerMethod": "POST",
            "oLanguage": { "sSearch": "Search stuff:"},
            "aoColumns": [
            {
                "mDataProp": null, //Note that I had a problem with this column being first because when the datatable loads, it automatically sorts based on the first column; since this column had a null value
                "bSortable": false, //it would pass that null value to the data call. I actually fixed this by modifying the code in InMemoryRepositories.cs so that if there was an error, it just returns the list
                "sClass": "control center",
                "sDefaultContent": '<img src="' + sOpenImageUrl + '">'
            },
            { "mDataProp": "trn_num" },
            { "mDataProp": "from_whse" },
            { "mDataProp": "to_whse" },
            { "mDataProp": "order_date" },
            { "mDataProp": "stat" },
            { "mDataProp": "CreateDate" },
            { "mDataProp": "weight" },
            { "mDataProp": "qty_packages" },
            { "mDataProp": "CreatedBy" },
            { "mDataProp": "UpdatedBy"}],
            "fnRowCallback": function (nRow, aData, iDisplayIndex, iDisplayIndexFull) {
                var objOrderdate = new Date(parseInt(aData.order_date.replace("/Date(", "").replace(")/", ""), 10));
                var objCreateDate = new Date(parseInt(aData.CreateDate.replace("/Date(", "").replace(")/", ""), 10));

                $('td:eq(4)', nRow).html(objOrderdate.getMonth() + 1 + "/" + objOrderdate.getDate() + "/" + objOrderdate.getFullYear());
                $('td:eq(6)', nRow).html(objCreateDate.getMonth() + 1 + "/" + objCreateDate.getDate() + "/" + objCreateDate.getFullYear());
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

        sOrderNo = oTable.fnGetData(rowIndex).trn_num; //get the order number of the current row

        sLineURL = document.URL.replace(/Search$/, ""); //uses a RegEx to remove the string 'Search' only if it is at the end of the URL 
        sLineURL = sLineURL + 'SearchTOLinesByOrder';

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
                        { "mDataProp": "trn_num", "sWidth": 30, fnRender: makeViewNotesBtn, "bSortable": false, "bSearchable": false },
                        { "mDataProp": "trn_line" },
                        { "mDataProp": "stat" },
                        { "mDataProp": "item" },
                        { "mDataProp": "trn_loc" },
                        { "mDataProp": "qty_shipped" },
                        { "mDataProp": "sch_rcv_date" },
                        { "mDataProp": "sch_ship_date" },
                        { "mDataProp": "from_whse" },
                        { "mDataProp": "to_whse" },
                        { "mDataProp": "CreatedBy" },
                        { "mDataProp": "CreateDate"}],
                "fnRowCallback": function (nRow, aData, iDisplayIndex, iDisplayIndexFull) {

                    if (aData.sch_rcv_date != null && aData.sch_rcv_date !== undefined) {
                        var objschrcvdate = new Date(parseInt(aData.sch_rcv_date.replace("/Date(", "").replace(")/", ""), 10));
                        $('td:eq(6)', nRow).html(objschrcvdate.getMonth() + 1 + "/" + objschrcvdate.getDate() + "/" + objschrcvdate.getFullYear());
                    }

                    if (aData.sch_ship_date != null && aData.sch_ship_date !== undefined) {
                        var objschshipdate = new Date(parseInt(aData.sch_ship_date.replace("/Date(", "").replace(")/", ""), 10));
                        $('td:eq(7)', nRow).html(objschshipdate.getMonth() + 1 + "/" + objschshipdate.getDate() + "/" + objschshipdate.getFullYear());
                    }

                    if (aData.CreateDate != null && aData.CreateDate !== undefined) {
                        var objCreateDate = new Date(parseInt(aData.CreateDate.replace("/Date(", "").replace(")/", ""), 10));
                        $('td:eq(11)', nRow).html(objCreateDate.getMonth() + 1 + "/" + objCreateDate.getDate() + "/" + objCreateDate.getFullYear());
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

/*var test = $("#dialog").dialog({
    autoOpen: false,
    position: 'center' ,
    title: 'EDIT',
    draggable: false,
    width : 300,
    height : 40, 
    resizable : false,
    modal : true
});*/

function loadDialog(sUrl)
{
    //test.load(sUrl, function() {$("#dialog").dialog("open")});
    //alert(sUrl);
    $("#dialog").load(sUrl).dialog({ modal: true }); 
};

function makeViewNotesBtn(oObj) {
    var sOrderNo = oObj.aData.trn_num;
    var sLineNo = oObj.aData.trn_line;

    return "<a href='" + sLineURL + '?&OrderNo=' + sOrderNo + '&LineNo=' + sLineNo + "' class='ViewNotes' title='View Notes'><img src='" + sOpenImageUrl + "' height='12' width='12'></a>";


    //var sHref = sLineURL + '?&OrderNo=' + sOrderNo + '&LineNo=' + sLineNo;
    //return "<a href=\"javascript:loadDialog('" + sHref + "')\" class='ViewNotes' title='View Notes'><img src='" + sOpenImageUrl + "' height='12' width='12'></a>";

    

};