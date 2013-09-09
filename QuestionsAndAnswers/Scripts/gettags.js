
    $(function () {
        //Subcribe to the tags
        $('#subcribes').click(function () {

            var emaildata = $("#emailinfo").val();
            var isauth = $("#isauth").val();
            var isverify = $("#isverify").val();
            //If there isn't emailaddress
            if (emaildata == "No") {
                //We redirect to email registration
                if(isauth=="yes")
                    document.getElementById('emaillink').click();
                //An error happaned
                else
                    document.getElementById('errorlink').click();
            }
            else if (emaildata != "No" && isverify == "no") {
                document.getElementById('verifylink').click();
            }
                //If there is emailaddress
            else {
                //Tags
                var mydata = $("#tags-input").val();
                $.ajax({
                    url: "/Home/SubcribeToTags",
                    type: "Get",
                    data: { tags: mydata },
                    success:
                        function (data) {
                            //There is a problem (bad tags)
                            if (data == "Problem") {
                                document.getElementById('errorlink').click();
                            }
                                //Everything is OK
                            else {
                                //Delete the old tag(s)
                                $("#sub_tag").empty();
                                //Append the new tag(s)
                                $("#sub_tag").append('<b><font color="#0266C8" size="3">' + data + '</font></b>');
                                //Call the JqueryReveal
                                document.getElementById('mylink').click();
                            }

                        }
                });
            }
        });


        var tiptip_con = '<p>' + $("#tiptip_content").val() + '</p>';
        $('#help').tipTip({
            defaultPosition: 'left',
            delay: 55,
            content: tiptip_con



        });
    });


$(function () {

    //Autocomplete for Title of question via Ajax
    $("#tags-input").bind("keydown", function (event) {
        if (event.keyCode === $.ui.keyCode.TAB &&
                $(this).data("autocomplete").menu.active) {
            event.preventDefault();
        }
    })
    $("#tags-input").autocomplete({
        source: function (request, response) {
            $.ajax({
                url: "/Home/FindTags",
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


        minLength: 0,




    });
});

