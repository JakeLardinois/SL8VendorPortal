var anOpen = [];
var oTable;


$(document).ready(function () {
    oTable = $('#objNotes').dataTable({
        "bProcessing": true,
        "bServerSide": true,
        "sAjaxSource": document.URL,
        "bFilter": false,   //hides the search box
        "bPaginate": false, //disables paging functionality
        "sDom": '<"top">rt<"bottom"flp><"clear">', //hides footer that displays 'showing record 1 of 1'...
        "sServerMethod": "POST",
        "aoColumns": [
            {
                "mDataProp": null, //Note that I had a problem with this column being first because when the datatable loads, it automatically sorts based on the first column; since this column had a null value
                "bSortable": false, //it would pass that null value to the data call. I actually fixed this by modifying the code in InMemoryRepositories.cs so that if there was an error, it just returns the list
                "sClass": "control center",
                "sDefaultContent": '<img src="' + sOpenImageUrl + '">'
            },
            { "mDataProp": "NoteDesc" },
            { "mDataProp": "CreatedBy" },
            { "mDataProp": "UpdatedBy" },
            { "mDataProp": "LastUpdated" }],
        "fnRowCallback": function (nRow, aData, iDisplayIndex, iDisplayIndexFull) {
            if (aData.LastUpdated != null && aData.LastUpdated !== undefined) {
                var objLastUpdated = new Date(parseInt(aData.LastUpdated.replace("/Date(", "").replace(")/", ""), 10));
                $('td:eq(4)', nRow).html(objLastUpdated.getMonth() + 1 + "/" + objLastUpdated.getDate() + "/" + objLastUpdated.getFullYear());
            }
        }
    });

    $('#objNotes tbody').on('click', 'td.control', function () {
        var nTr;
        var i;
        var rowIndex;
        var nDetailsRow;
        var sLineTableName;
        var objRow;

        nTr = this.parentNode;
        i = $.inArray(nTr, anOpen);

        rowIndex = oTable.fnGetPosition(nTr); //get the index of the current row
        sLineTableName = 'dtLineTable' + rowIndex;

        //sOrderNo = oTable.fnGetData(rowIndex).NoteContent; //get the order number of the current row
        objRow = oTable.fnGetData(rowIndex);

        if (i === -1) { //the datatable is opening the row...
            $('img', this).attr('src', sCloseImageUrl);
            nDetailsRow = oTable.fnOpen(nTr, fnFormatDetails(objRow), 'details');
            $('div.innerDetails', nDetailsRow).slideDown();
            anOpen.push(nTr);

        }
        else {
            $('img', this).attr('src', sOpenImageUrl);
            $('div.innerDetails', $(nTr).next()[0]).slideUp(function () {
                oTable.fnClose(nTr);
                anOpen.splice(i, 1);
            });
        }
    });
    // Open this PartialView as a modal dialog box.
    /*$('#objNotes').dialog({
        modal: true,
        resizable: false,
        width: 500
    });*/
});


function fnFormatDetails(objRow) {

    var sOut =
            '<div class="innerDetails">' +
                '<table>' +
                    '<tbody>' +
                        '<tr>' +
                            '<td>' +
                            objRow.HTMLNoteContent +
                            '</td>' +
                        '</tr>' +
                    '</tbody>' +
                '</table>' +
            '</div>';

    return sOut;
}