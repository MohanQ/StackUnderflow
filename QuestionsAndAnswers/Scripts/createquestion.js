
//Helper method for autocomplete
function split(val) {
    return val.split(/,\s*/);
}

//Helper method for autocomplete
function extractLast(term) {
    return split(term).pop();
}

$(function () {

 //Autocomplete for Tags via Ajax
    $("#tags").bind("keydown", function (event) {
        if (event.keyCode === $.ui.keyCode.TAB &&
                $(this).data("autocomplete").menu.active) {
            event.preventDefault();
        }
    })
    $("#tags").autocomplete({
        source: function (request, response) {
            $.ajax({
                url: "/Home/FindTags",
                data: { prefix: extractLast(request.term) },
                success: function (data) {
                    response($.map(data, function (item) {
                        return {
                            label: item.Value,
                            value: item.Value,
                            key: item.Key
                        };
                    }));
                }
            });
        },


        focus: function () {
            // prevent value inserted on focus
            return false;
        },


        minLength: 0,

        select: function (event, ui) {
            var terms = split(this.value);
            // remove the current input
            terms.pop();
            // add the selected item
            terms.push(ui.item.value);
            // add placeholder to get the comma-and-space at the end
            terms.push("");
            this.value = terms.join(", ");
            return false;
        }

    });
});


$(function () {

    //Autocomplete for Title of question via Ajax
    $("#Title").bind("keydown", function (event) {
        if (event.keyCode === $.ui.keyCode.TAB &&
                $(this).data("autocomplete").menu.active) {
            event.preventDefault();
        }
    })
    $("#Title").autocomplete({
        source: function (request, response) {
            $.ajax({
                url: "/Home/FindQuestion",
                data: { prefix: request.term },
                success: function (data) {
                    response($.map(data, function (item) {
                        return {
                            label: item.Value,
                            value: item.Value,
                            key: item.Key
                        };
                    }));
                }
            });
        },


        focus: function () {
            event.preventDefault();
        },


        minLength: 0,

        select: function (event, ui) {
            event.preventDefault();
            var link = "GetQuestion/" + ui.item.key;
            window.open(link, '_blank');

        }
    });
});

//CKEDITOR
window.onload = function () {
    CKEDITOR.replace('content_editor');
    CKEDITOR.config.enterMode = CKEDITOR.ENTER_BR;
};


//Copy the content of hidden by validate
$("#form_submit").click(function () {
    var editor_data = CKEDITOR.instances.content_editor.getData();
    $("#Content").val(editor_data);
}
);


//Enable hidden-validate (jquery validate)
$(function () {
    var validatorSettings = $.data($('form')[0], 'validator').settings;
    validatorSettings.ignore = "";

    //There is validate only by submit
    var validator = $("form").data("validator");
    if (validator) {
        validator.settings.onkeyup = false; // disable validation on keyup
        validator.settings.onclick = false;
        validator.settings.onfocusout = false;
    }
});

//Validate method on client side
function validate() {
    //Delete content of validate div
    $("#tagvalidate").empty();

    //Check content of inputs  
    var tagContent = $.trim($(" #tags").val());
    var isValid = true;
    //Delete space
    if (tagContent == '') {
        var val_content = '<font color="red">' + $("#tv1").val() + '</font></br>';
        $("#tagvalidate").append(val_content);
        isValid = false;
    }
    //Check all of the tags
    var tags = tagContent.split(',');
    taglength = tags.length;
    if (tags[taglength - 1] == "")
        taglength = taglength - 1;
    if ((taglength) > 5) {
        var val_content = '<font color="red">' + $("#tv2").val() + '</font></br>';
        $("#tagvalidate").append(val_content);
        isValid = false;
    }
    //One by one checking for tags
    for (var i = 0; i < taglength; i++) {
        if (tags[i].length > 25) {
            isValid = false;
            var val_content = '<font color="red">' + $("#tv3").val() + '</font></br>';
            $("#tagvalidate").append(val_content);
            break;
        }            
    }

    if (isValid) {
        return true;
    }
    if (!isValid) {
        return false
    }
}



