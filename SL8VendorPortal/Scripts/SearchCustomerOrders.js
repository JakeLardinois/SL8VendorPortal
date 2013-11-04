var anOpen = [];
var oTable;
var strMinDate = dateFormat(new Date(1900, 0, 1), "mm-dd-yyyy"); // '1/1/1900';


TableTools.BUTTONS.download = {
    "sAction": "text",
    "sTag": "default",
    "sFieldBoundary": "",
    "sFieldSeperator": "\t",
    "sNewLine": "<br>",
    "sToolTip": "",
    "sButtonClass": "DTTT_button_text",
    "sButtonClassHover": "DTTT_button_text_hover",
    "sButtonText": "Download",
    "mColumns": "all",
    "bHeader": true,
    "bFooter": true,
    "sDiv": "",
    "fnMouseover": null,
    "fnMouseout": null,
    "fnClick": function (nButton, oConfig) {
        var oParams = this.s.dt.oApi._fnAjaxParameters(this.s.dt);
        var iframe = document.createElement('iframe');
        iframe.style.height = "0px";
        iframe.style.width = "0px";
        iframe.src = oConfig.sUrl + "?" + $.param(oParams);
        document.body.appendChild(iframe);
    },
    "fnSelect": null,
    "fnComplete": null,
    "fnInit": null
};

function FormatDate(objDate, blnIncludeTime) {
    var objDateProcessed = new Date(parseInt(objDate.replace("/Date(", "").replace(")/", ""), 10));

    //return objDateProcessed.getMonth() + 1 + "/" + objDateProcessed.getDate() + "/" + objDateProcessed.getFullYear();
    //return dateFormat(objDateProcessed, "dddd, mmmm dS, yyyy, h:MM:ss TT"); //returns Thursday, November 1st, 2012, 12:00:00 AM
    if (blnIncludeTime)
        return dateFormat(objDateProcessed, "mm-dd-yyyy HH:MM:ss"); //returns 11-01-2012 12:30:00
    else
        return dateFormat(objDateProcessed, "mm-dd-yyyy"); //returns  11-01-2012
}

$(document).ready(function () {
    var oTimerId;


    $.editable.addInputType('autogrow', {   //adds the autogrow plugin for editing notes
        element: function (settings, original) {
            var textarea = $('<textarea />');
            if (settings.rows) {
                textarea.attr('rows', settings.rows);
            } else {
                textarea.height(settings.height);
            }
            $(this).append(textarea);
            return (textarea);
        },
        plugin: function (settings, original) {
            $('textarea', this).autogrow(settings.autogrow);
        }
    });

    //note that these statements have nothing to do with autogrow in my datatables. I put the below here so that I could get autogrow to work on my frmAddVendorRequest; autogrow for datatables is handled in the .MakeEditable instantiation
    //$('textarea').autogrow(); // will apply autogrow to all textareas...
    $('.MyAutogrow').autogrow(); //applies autogrow to any textarea with statement class="MyAutogrow"...

    oTable = $('#objItems').dataTable({
        "bProcessing": true,
        "bServerSide": true,
        "sDom": 'T<"clear">Rlfrtip', //Enables column reorder with resize. 'T<"clear"> adds the 'download' button
        "oTableTools": {
            "aButtons": [
                {
                    "sExtends": "download",
                    "sButtonText": "Excel Download",
                    "sUrl": sPrintCOUrl // "/generate_csv.php"
                }
            ]
        },
        "sScrollX": "100%",
        "bJQueryUI": true,
        "sPaginationType": "full_numbers",
        "sAjaxSource": document.URL,
        "fnServerData": function (sSource, aoData, fnCallback, oSettings) {

            window.clearTimeout(oTimerId);
            oTimerId = window.setTimeout(function () {
                oSettings.jqXHR = $.ajax({
                    "dataType": 'json',
                    "type": "POST",
                    "url": sSource,
                    "data": aoData,
                    "success": fnCallback
                });
            }, 1000)
        },
        "sServerMethod": "POST",
        "oLanguage": { "sSearch": "Search Order #s:" },
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
            {
                "mDataProp": "order_date",
                "fnRender": function (oObj) {
                    if (oObj.aData.order_date != null && oObj.aData.order_date !== undefined) {
                        return FormatDate(oObj.aData.order_date);
                    }
                }
            },
            { "mDataProp": "taken_by" },
            { "mDataProp": "ship_code" },
            { "mDataProp": "weight" },
            { "mDataProp": "qty_packages" },
            { "mDataProp": "slsman" },
            {
                "mDataProp": "eff_date",
                "fnRender": function (oObj) {
                    if (oObj.aData.eff_date != null && oObj.aData.eff_date !== undefined) {
                        return FormatDate(oObj.aData.eff_date);
                    }
                    else
                        return strMinDate;
                }
            },
            {
                "mDataProp": "exp_date",
                "fnRender": function (oObj) {
                    if (oObj.aData.exp_date != null && oObj.aData.exp_date !== undefined) {
                        return FormatDate(oObj.aData.exp_date);
                    }
                    else
                        return strMinDate;
                }
            },
            { "mDataProp": "whse" },
            { "mDataProp": "ship_partial" },
            { "mDataProp": "ship_early" },
            {
                "mDataProp": "projected_date",
                "fnRender": function (oObj) {
                    if (oObj.aData.projected_date != null && oObj.aData.projected_date !== undefined) {
                        return FormatDate(oObj.aData.projected_date);
                    }
                    else
                        return strMinDate;
                }
            },
            {
                "mDataProp": "RecordDate",
                "fnRender": function (oObj) {
                    if (oObj.aData.RecordDate != null && oObj.aData.RecordDate !== undefined) {
                        return FormatDate(oObj.aData.RecordDate);
                    }
                }
            },
            { "mDataProp": "CreatedBy" },
            { "mDataProp": "UpdatedBy" },
            {
                "mDataProp": "CreateDate",
                "fnRender": function (oObj) {
                    if (oObj.aData.CreateDate != null && oObj.aData.CreateDate !== undefined) {
                        return FormatDate(oObj.aData.CreateDate);
                    }
                }
            }]
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
            nDetailsRow = oTable.fnOpen(nTr, GetLineTableHTML(oTable, nTr, sLineTableName), 'details');
            $('div.innerDetails', nDetailsRow).slideDown();
            anOpen.push(nTr);


            var tInnerTable = $('#' + sLineTableName).dataTable({ //when referencing the table for it's api, it always wants to be prefaced with #
                "bProcessing": true,
                "sDom": "Rlfrtip", //Enables column reorder with resize
                "bFilter": false,   //hides the search box
                "bPaginate": false, //disables paging functionality
                "bServerSide": true,
                "sAjaxSource": sLineURL + '?&OrderNo=' + sOrderNo, //pass the order number to the orderline url as a querystring; note query string variables are delimited by &
                "sDom": '<"top">rt<"bottom"flp><"clear">', //hides the filter box and the 'showing recordno of records' message
                "sServerMethod": "POST", "aoColumns": [
                        {//Can't have a bound column followed by a null column, so this one has to be null
                            fnRender: makeViewNotesBtn,
                            "mDataProp": null,
                            "sWidth": 30,
                            "bSortable": false,
                            "bSearchable": false,
                            "sDefaultContent": '<img src="' + sOpenImageUrl + '">'
                        },
                        {  //I can't have the same mDataProp used twice on the datatable, and when I used null I kept getting a 'Requested unknown parameter '0' datatables error
                            fnRender: makeVendorRequestBtn,
                            "mDataProp": null,
                            "sWidth": 30,
                            "bSortable": false,
                            "bSearchable": false,
                            "sDefaultContent": '<img src="' + sOpenImageUrl + '">'//adding sDefaultContent solved the error from having a null dataprop
                        },
                        {  //I can't have the same mDataProp used twice on the datatable, and when I used null I kept getting a 'Requested unknown parameter '0' datatables error
                            fnRender: makeViewCustAddrBtn,
                            "mDataProp": null,
                            "sWidth": 30,
                            "bSortable": false,
                            "bSearchable": false,
                            "sDefaultContent": '<img src="' + sOpenImageUrl + '">'//adding sDefaultContent solved the error from having a null dataprop
                        },
                        { "mDataProp": "item" },
                        { "mDataProp": "co_line" },
                        { "mDataProp": "co_release" },
                        { "mDataProp": "qty_ordered" },
                        { "mDataProp": "qty_shipped" },
                        {
                            "mDataProp": "due_date",
                            "fnRender": function (oObj) {
                                if (oObj.aData.due_date != null && oObj.aData.due_date !== undefined) {
                                    return FormatDate(oObj.aData.due_date);
                                }
                            }
                        },
                        {
                            "mDataProp": "promise_date",
                            "fnRender": function (oObj) {
                                if (oObj.aData.promise_date != null && oObj.aData.promise_date !== undefined) {
                                    return FormatDate(oObj.aData.promise_date);
                                }
                                else
                                    return strMinDate;
                            }
                        },
                        {
                            "mDataProp": "CreateDate",
                            "fnRender": function (oObj) {
                                if (oObj.aData.CreateDate != null && oObj.aData.CreateDate !== undefined) {
                                    return FormatDate(oObj.aData.CreateDate);
                                }
                            }
                        }]

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


});

function loadDialog(sUrl) {
    var oNotesTable;

    /*The below sets the html in the div to the proper table with ID objNotes so datatables can instantiate it. Before I did this I had a problem with datatables erroring out because
    I was trying to initialize the same data table a second time*/
    $("#NotesDialogDiv").html(GetNoteTableHTML());

    oNotesTable = $('#objNotes').dataTable({
        "bJQueryUI": true,
        "sDom": "Rlfrtip", //Enables column reorder with resize
        "bProcessing": true,
        "bServerSide": true,
        "sAjaxSource": sUrl, //document.URL,
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
            {
                "mDataProp": "LastUpdated"
                //"fnRender": function (oObj) {
                //    if (oObj.aData.LastUpdated != null && oObj.aData.LastUpdated !== undefined) {
                //        return FormatDate(oObj.aData.LastUpdated);
                //    }
                //}
            }],
        "fnRowCallback": function (nRow, aData, iDisplayIndex, iDisplayIndexFull) {
            /*This needed to be reinstated because fnRender changed the value that was sent to the controller and broke the note updating functionality due to the time
            being truncated off by the FormatDate function; see NotesController's UpdateNote method for details*/
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

        rowIndex = oNotesTable.fnGetPosition(nTr); //get the index of the current row
        sLineTableName = 'dtLineTable' + rowIndex;

        objRow = oNotesTable.fnGetData(rowIndex);

        if (i === -1) { //the datatable is opening the row...
            $('img', this).attr('src', sCloseImageUrl);
            nDetailsRow = oNotesTable.fnOpen(nTr, GetNoteDetailHTML(objRow), 'details');
            $('div.innerRowDetails', nDetailsRow).slideDown();
            anOpen.push(nTr);

            //Creates the editable, autogrowing control that asynchronously updates the database via Ajax; See SearchTransferOrders.js for detailed comments.
            $('.EditNotes').editable(sNotesUpdateUrl, {
                id: 'PostedNoteData',
                name: 'NoteContent', 
                submitdata: { SpecificNoteToken: objRow.SpecificNoteToken, NoteDesc: objRow.NoteDesc, LastUpdated: objRow.LastUpdated },
                type: "autogrow",
                submit: 'OK',
                cancel: 'Cancel',
                tooltip: "Click to edit.",
                onblur: "ignore",
                indicator: '<img src="' + sProgressImageUrl + '">',
                data: function (value, settings) {
                    /* Convert <br> to newline. */
                    //alert(value);
                    var retval = value.replace(/<br[\s\/]?>/gi, '\n');
                    return retval;
                },
                callback: function (value, settings) {
                    sValue = JSON.parse(value);

                    if (sValue.Success) {
                        objRow.LastUpdated = sValue.LastUpdate
                        oNotesTable.fnUpdate(objRow, rowIndex);
                    }
                    else {
                        alert('The edits failed to save');
                    }
                    $('#NoteEditorDiv').html(sValue.HTMLNoteContent);
                }
            });
        }
        else {
            $('img', this).attr('src', sOpenImageUrl);
            $('div.innerRowDetails', $(nTr).next()[0]).slideUp(function () {
                oNotesTable.fnClose(nTr);
                anOpen.splice(i, 1);
            });
        }
    });

    // Open this Datatable as a modal dialog box.
    $('#NotesDialogDiv').dialog({
        modal: true,
        resizable: true,
        position: 'top',
        width: 'auto',
        autoResize: true,
        title: 'Customer Order Notes'
    });
};

function makeViewNotesBtn(oObj) {
    var sOrderNo = oObj.aData.co_num;
    var sLineNo = oObj.aData.co_line;
    var sReleaseNo = oObj.aData.co_release;
    var sHref


    //return "<a href='" + sLineURL + '?&OrderNo=' + sOrderNo + '&LineNo=' + sLineNo + '&ReleaseNo=' + sReleaseNo + "' class='ViewNotes' title='View Notes'><img src='" + sOpenImageUrl + "' height='12' width='12'></a>";

    sHref = sCONotesUrl + '?&OrderNo=' + sOrderNo + '&LineNo=' + sLineNo + '&ReleaseNo=' + sReleaseNo; //generate the query string
    return "<a href=\"javascript:loadDialog('" + sHref + "')\" class='ViewNotes' title='View Syteline Notes'><img src='" + sOpenImageUrl + "' height='12' width='12'></a>";
}

function makeVendorRequestBtn(oObj) {
    var sOrderNo = oObj.aData.co_num;
    var sLineNo = oObj.aData.co_line;
    var sReleaseNo = oObj.aData.co_release;
    var sItemID = oObj.aData.item;
    var sHref

    sHref = sVendorRequestsUrl + '?&OrderNo=' + sOrderNo + '&LineNo=' + sLineNo + '&ReleaseNo=' + sReleaseNo + '&ItemID=' + sItemID + '&RequestType=CO'; //generate the query string
    return "<a href=\"javascript:loadVendorRequestDialog('" + sHref + "')\" class='Process' title='View Vendor Requests'><img src='" + sOpenImageUrl + "' height='10' width='10'></a>";
};

function makeViewCustAddrBtn(oObj) {
    var sCustNo = oObj.aData.cust_num;
    var sSeqNo = oObj.aData.cust_seq;
    var sHref


    sHref = sCustAddrUrl + '?&CustNo=' + sCustNo + '&SeqNo=' + sSeqNo; //generate the query string
    return "<a href=\"javascript:loadAddrDialog('" + sHref + "')\" class='ViewNotes' title='View Address'><img src='" + sOpenImageUrl + "' height='12' width='12'></a>";
}

function loadAddrDialog(sUrl) {
    var strAddr;
    var sHTML


    $.ajaxSetup({ async: false }); 
    $.post(sUrl, {}, function (data) {//Gets the address string from the controller
        strAddr = data;
    });
    $.ajaxSetup({ async: true }); //Sets ajax back up to synchronous

    sHTML =
        '<table>' +
            '<tbody>' +
                '<tr>' +
                    '<td>' +
                        '<div>' +
                            strAddr +
                        '</div>' +
                    '</td>' +
                '</tr>' +
            '</tbody>' +
        '</table>';

    $("#AddressDialogDiv").html(sHTML);

    // Open this Datatable as a modal dialog box.
    $('#AddressDialogDiv').dialog({
        modal: true,
        resizable: true,
        position: 'center',
        width: 'auto',
        autoResize: true,
        title: 'Address'
    });
}


function loadVendorRequestDialog(sUrl) {
    var oVRequestTable;

    //This is a cool little chunk of code that I use to get the values of parameters from the querystring sUrl. For sUrl='/VendorRequests/VendorRequests?&OrderNo=T000000267&LineNo=1' I can call it like GET("OrderNo")[0] to get 'T000000267'
    //or I can call it like data = GET("OrderNo","LineNo"); and get data[0]= 'T000000267' and data[1]= '1'.
    function GET() {
        var data = [];
        for (x = 0; x < arguments.length; ++x)
            data.push(sUrl.match(new RegExp("/\?".concat(arguments[x], "=", "([^\n&]*)")))[1])
        return data;
    }

    //This is a way to create a reusable column, so you don't have to redefine the same variables on each column instantiation of makeEditable()
    //note that all options that are available for editable() from jeditable are available here...
    defaultEditable = {
        tooltip: 'Click to edit',
        type: 'select', //was 'textarea' in the example which called my autogrowing textbox...
        onblur: 'submit',
        data: UserWarehouses()
    }

    /*The below sets the html in the div to the proper table with ID objNotes so datatables can instantiate it. Before I did this I had a problem with datatables erroring out because
    I was trying to initialize the same data table a second time*/
    $("#VendorRequestDialogDiv").html(GetVRTableHTML()); //Just a hidden empty div on Search.cshtml that the popup uses...


    oVRequestTable = $('#objVendorRequest').dataTable({
        "bJQueryUI": true,
        "sDom": "Rlfrtip", //Enables column reorder with resize
        "bProcessing": true,
        "bServerSide": true,
        "bFilter": false,   //hides the search box
        "bPaginate": false, //disables paging functionality
        "sDom": '<"top">rt<"bottom"flp><"clear">', //hides footer that displays 'showing record 1 of 1'...
        "sAjaxSource": sUrl, //document.URL,
        "sServerMethod": "POST",
        "aoColumns": [
            {   //I had to add this column to the table otherwise it was passing the button href to my update Url; Datatables wants the first column to be a primary key...
                "mDataProp": null, //When posting, there was no value being passed to the 'id' parameter that datatables datatables automatically posts; THE ONLY WAY that I could get a the key value (column 'ID' in my database) to populate to the id parameter
                "sDefaultContent": "ID",    //Was to set mDataProp to null, set sDefaultContent to some arbitrary value (I used 'ID') and then populate the cell with the 'ID' property of the row which maps to my ID column in my database..
                "bVisible": false, //Setting this column as hidden removed it from the display and negated the need to include it in my makeEditable aoColumns array, however I still had to add the column to GetVRTableHTML()
                "fnRender": function (oObj) {
                    return oObj.aData["ID"]; //accessing the ID column of the specific row
                }
            },
            {
                "mDataProp": null, //Note that I had a problem with this column being first because when the datatable loads, it automatically sorts based on the first column; since this column had a null value
                "bSortable": false, //it would pass that null value to the data call. I actually fixed this by modifying the code in InMemoryRepositories.cs so that if there was an error, it just returns the list
                "sClass": "control center",
                "sDefaultContent": '<img src="' + sOpenImageUrl + '">'
            },
            { "mDataProp": "RequestCategoryCode" },
            { "mDataProp": "Qty" },
            {
                "mDataProp": "QtyLoss",
                "bVisible": false
            },
            { "mDataProp": "SourceWarehouse" },
            { "mDataProp": "Processed" },
            {
                "mDataProp": "OrderNo", //I don't show this row on the datatable, but I need it here so that I can populate it on my frmAddVendorRequest
                "bVisible": false
            },
            {
                "mDataProp": "LineNo", //I don't show this row on the datatable, but I need it here so that I can populate it on my frmAddVendorRequest
                "bVisible": false
            },
            {
                "mDataProp": "Notes", //I don't show this row on the datatable, but I need it here so that I can populate it on my frmAddVendorRequest
                "bVisible": false
            },
            {
                "mDataProp": "RequestCategoryID", //I don't show this row on the datatable, but I need it here so that I can populate it on my frmAddVendorRequest
                "bVisible": false
            },
            {
                "mDataProp": "ReleaseNo", //I don't show this row on the datatable, but I need it here so that I can populate it on my frmAddVendorRequest
                "bVisible": false
            }]
        }).makeEditable({ //I kept getting the error "Object doesn't support property or method 'live'" until I changed line 1260 of jquery.dataTables.editable.js 'live' method to 'on'
        sUpdateURL: sUpdateVRUrl,
        sAddURL: sAddVRUrl,
        sDeleteURL: sDeleteVRUrl,
        fnOnAdding: function () {//This event gets fired right before the form is submitted to the server. I can use this time to populate the variables I need to add a new record into the database such as OrderNo, OrderLine, OrderRelease, etc.
            //In order for this to work I needed to add the columns to the datatable and then hide them, put the hiddent textboxes on the frmAddVendorRequest form with the proper rel="#" attribute (where # is the column order
            //That they were added in the column instantiation. I then populate thier values here...

            //uses the GET function defined above and retrieves the values from the querystring...
            var dataValues = GET("OrderNo", "LineNo", "ReleaseNo", "ItemID");
            document.getElementById('OrderNo').value = dataValues[0];
            document.getElementById('LineNo').value = dataValues[1];
            document.getElementById('ReleaseNo').value = dataValues[2];
            document.getElementById('Item').value = dataValues[3];
            document.getElementById('RequestCategoryID').value = 0; //Customer Orders have a request category of 0

            return true;
        },
        sAddNewRowFormId: "frmAddVendorRequest", //specifies the ID of the form that will be used to add a new row...
        "oAddNewRowFormOptions": { //These options are the same/taken from the jquery.dialog() options http://api.jqueryui.com/dialog/
            "title": "Add new Vendor Request", //This can also be done by setting the attribute 'title = "Add new Vendor Request"' in the form tag
            "height": 450,
            "width": 700
        },
        aoColumns: [
            null,
            {
                type: 'select',
                onblur: 'submit',  //values can be "ignore", "submit", or "cancel"
                //data: "{'COShipment':'COShipment', 'COLateRequest':'COLateRequest'}",
                data: RequestCategories()
                //submit: 'Save changes'//shows a submit button
            },
            {},
            //{}, //not needed once I removed LossQty
            $.extend({}, defaultEditable, { indicator: 'Saving...' }) //create a column using defaultEditable value that is listed above, also passes an 'indicator' parameter that is specific to this instance...
        ]
    });

    $('#objVendorRequest tbody').on('click', 'td.control', function () {

        var nTr;
        var i;
        var rowIndex;
        var nDetailsRow;
        var sLineTableName;
        var objRow;

        nTr = this.parentNode;
        i = $.inArray(nTr, anOpen);


        rowIndex = oVRequestTable.fnGetPosition(nTr); //get the index of the current row
        sLineTableName = 'dtProcessTable' + rowIndex;

        objRow = oVRequestTable.fnGetData(rowIndex);

        if (i === -1) { //the datatable is opening the row...
            $('img', this).attr('src', sCloseImageUrl);
            nDetailsRow = oVRequestTable.fnOpen(nTr, GetVRNoteDetailHTML(objRow), 'details');
            $('div.innerRowDetails', nDetailsRow).slideDown();
            anOpen.push(nTr);

            $('.EditRequestNotes').editable(sVRNotesUpdateUrl, { //make the note detail editable...
                id: 'PostedNoteData',  //gives the label 'id' the label 'PostedNoteData'
                name: 'NoteContent', //labels the updated data as NoteContent instead of 'value'
                submitdata: { ID: objRow.ID, notes: objRow.notes },
                type: "autogrow", //specifies to use the autogrow input specified above
                submit: 'OK', //Adds the OK button that submits post
                cancel: 'Cancel', //adds the Cancel button that cancels the edit
                tooltip: "Click to edit.",
                onblur: "ignore", //what happens when user clicks out of textarea values can be "ignore", "submit", or "cancel"
                indicator: '<img src="' + sProgressImageUrl + '">', //This is what shows while ajax is processing. Could also just be a text message like 'Saving...'
                data: function (value, settings) { //This gets fired when the control goes into edit mode
                    /* Convert <br> to newline. */
                    var retval = value.replace(/<br[\s\/]?>/gi, '\n');
                    return retval;
                },
                callback: function (value, settings) {//This is fired after the control uses ajax to save the data to the server and the JSON Result is recieved back from the server
                    sValue = JSON.parse(value); //puts the JSON data into objects form so they can be accessed like sValue.LastUpdate, etc.

                    if (sValue.Success) {}
                    else {
                        alert('The edits failed to save');
                    }
                    $('#VRNoteEditorDiv').html(sValue.HTMLNotes); //Sets the NoteEditorDiv to the html of the newly saved Notes Object's HTML property
                    //window.location.reload();
                }
            });
        }
        else {
            $('img', this).attr('src', sOpenImageUrl);
            $('div.innerRowDetails', $(nTr).next()[0]).slideUp(function () {
                oVRequestTable.fnClose(nTr);
                anOpen.splice(i, 1);
            });
        }
    });

    //This function needed to be in the same Block as the calling function in the oProcessTable.makeEditable() column call, else I would get the error 'SCRIPT5002: Function expected'. Its almost like the 
    //Script file gets detached from the DOM or something and then the column instantiation can't find the AllWarehouses() function... This took me a day to figure out!
    function UserWarehouses() {
        var sValue;
        $.post(sUserWhsesUrl, {}, function (data) {
            UserWarehouses = FormatJSON(data);
        });
        return UserWarehouses;
    }

    //This function gets all the Transfer Order Request Categories from the Database
    function RequestCategories() {
        var sValue;

        //I modified the below so that I didn't have to specify a different controller action for CO, PO, and TO request categories. I pass the RequestCategory ID in a query string so the controllerselects the correct one
        $.post(sRequestCatUrl + '?&RequestCategoryID=0', {}, function (data) {
            RequestCategories = FormatJSON(data);
        });
        return RequestCategories;
    }

    //The JSON must be in the a format such as "{'TOReciept':'TOReciept', 'TOShipment':'TOShipment'}" This function does that
    function FormatJSON(x) {
        var orig = x;
        var stgify = JSON.stringify(orig);
        var splitchar = ['\",\"', '\"],[\"', '[', ']', '\"'];
        var joinchar = ['\':\'', '\', \'', '', '', '\''];

        //This is the loop that does the replacement of the above characters
        for (i = 0; i < 5; i++) {
            //alert("Split Char: " + splitchar[i] + "\r\n" + "Join Char: " + joinchar[i]);
            stgify = stgify.split(splitchar[i]);
            tmp = stgify.join(joinchar[i]);
            stgify = tmp;
        }

        stgify = "{" + stgify + "}";
        var finalEdit = stgify;
        return finalEdit;
    }

    // Open this Datatable as a modal dialog box.
    $('#VendorRequestDialogDiv').dialog({
        modal: true,
        resizable: true,
        position: 'top',
        width: 'auto',
        autoResize: true,
        title: 'Customer Order Requests'
    });
};