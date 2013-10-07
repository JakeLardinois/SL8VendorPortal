(function ($) {
    jQuery.fn.dataTableWithFilter = function (settings) {
        // alias the original jQuery object passed in since there is a possibility of multiple dataTables and search containers on a single page.
        // If we don't do this then we run the risk of having the wrong jQuery object before forcing a dataTable.fnDraw() call
        var $dataTable = this,
        searchCriteria = [],
        filterOptions = settings.filterOptions,
        // retrieves all inputs that we want to filter by in the searchContainer
        $searchContainerInputs = $('#' + filterOptions.searchContainer).find('input[type="text"],input[type="radio"],input[type="checkbox"],select,textarea');
        // remove the filterOptions object from the object literal (json) that will be passed to dataTables
        delete settings.filterOptions;
        if (filterOptions === undefined) {
            throw {
                name: 'filterOptionsUndefinedError',
                message: 'Please define a filterOptions property in the object literal'
            };
        }
        if (filterOptions.searchButton === undefined) {
            throw {
                name: 'searchButtonUndefinedError',
                message: 'Please define a searchButton in the filterOptions'
            };
        }
        if (filterOptions.clearSearchButton === undefined) {
            throw {
                name: 'clearSearchButtonUndefinedError',
                message: 'Please define a clearSearchButton in the filterOptions'
            };
        }
        if (filterOptions.searchContainer === undefined) {
            throw {
                name: 'searchContainerUndefinedError',
                message: 'Please define a searchContainer in the filterOptions'
            };
        }
        $searchContainerInputs.keypress(function (e) {
            if (e.keyCode === 13) {
                // if an enter key was pressed on one of our inputs then force the searchButton click event to happen
                $("#" + filterOptions.searchButton).click();
            }
        });
        $("#" + filterOptions.searchButton).click(function () {
            searchCriteria = [];
            var searchContainer = $("#" + filterOptions.searchContainer);
            searchContainer.find('input[type="text"][value!=""],input[type="radio"]:checked,input[type="checkbox"]:checked,textarea[value!=""],select[value!=""]').each(function () {
                // all textboxes, radio buttons, checkboxes, textareas, and selects that actually have a value associated with them
                var element = $(this), value = element.val();
                if (typeof value === "string") {
                    searchCriteria.push({ "name": element.attr("name"), "value": value });
                }
                else if (Object.prototype.toString.apply(value) === '[object Array]') {
                    // multi select since it has an array of selected values
                    var i;
                    for (i = 0; i < value.length; i++) {
                        searchCriteria.push({ "name": element.attr("name"), "value": value[i] });
                    }
                }
            });
            // force dataTables to make a server-side request
            $dataTable.fnDraw();
        });
        $("#" + filterOptions.clearSearchButton).click(function () {
            searchCriteria = [];
            $searchContainerInputs.each(function () {
                var $input = $(this),
                tagName = this.tagName.toLowerCase();
                if (tagName === "input") {
                    var type = $input.attr("type").toLowerCase();
                    if (type === "checkbox"
                    || type === "radio") {
                        $input.removeAttr("checked");
                    }
                    else if (type === "text") {
                        $input.val("");
                    }
                }
                else if (tagName === "select") {
                    if ($input.attr("multiple") !== undefined) {
                        $input.val([]);
                    }
                    else {
                        $input.val("");
                    }
                }
                else if (tagName === "textarea") {
                    $input.val("");
                }
            });
            $dataTable.fnDraw();
        });
        settings.fnServerParams = function (aoData) {
            var i;
            for (i = 0; i < searchCriteria.length; i++) {
                // pushing each name/value pair that was found from the searchButton click event in to the aoData array
                // which will be sent to the server in the request
                aoData.push(searchCriteria[i]);
            }
        };
        return $dataTable.dataTable(settings);
    };
} (jQuery));
