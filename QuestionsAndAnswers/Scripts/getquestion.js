
$(function () {
    var sortby = "votes";
    $("#orderbyvotes").parent().addClass("active");

    loadAnswers();

    var oldnumberofanswers;
    var numberofanswers;

    loadNumberOfAnswers();

    setInterval(function () { checkanswers() }, 5000);

    function checkanswers() {
        loadNumberOfAnswers();
        if (!isNaN(numberofanswers) && !isNaN(oldnumberofanswers)) {
            if (numberofanswers > 0) {
                $('#answersheader').show();
            }
            if (oldnumberofanswers != numberofanswers) {
                $('#newanswer').show(500);
            }
        }
        oldnumberofanswers = numberofanswers;
    }

    function loadNumberOfAnswers() {
        var questionid = $("#questionid").val();
        $.ajax({
            url: "/Home/GetNumberOfAnswersForOneQuestion/",
            data: { id: questionid},
            success: function (result) {
                numberofanswers = result;
                if (numberofanswers > 0) {
                    $('#answersheader').show();
                }
            }
        });
    }

    $('#newanswer').click(function () {
        $('#newanswer').hide();
        loadAnswers();
    });

    $("#orderbyvotes").click(function(){
        sortby = "votes";
        $("#orderbytime").parent().removeClass("active");
        $("#orderbyreversedtime").parent().removeClass("active");
        $("#orderbyvotes").parent().addClass("active");
        loadAnswers();
    });
    $("#orderbyreversedtime").click(function () {
        sortby = "reversedtime";
        $("#orderbyvotes").parent().removeClass("active");
        $("#orderbytime").parent().removeClass("active");
        $("#orderbyreversedtime").parent().addClass("active");
        loadAnswers();
    });
    $("#orderbytime").click(function () {
        sortby = "time";
        $("#orderbyreversedtime").parent().removeClass("active");
        $("#orderbyvotes").parent().removeClass("active");
        $("#orderbytime").parent().addClass("active");
        loadAnswers();
    });

    function loadAnswers() {
        var questionid = $("#questionid").val();
        $.ajax({
            url: "/Home/GetAnswersForAQuestion/",
            data: { id: questionid, sortby: sortby },
            success: function (result) {
                $('#answers').html(result);
            }
        });
    }
    
    $('#formid').submit(function () {
        if ($(this).valid()) {
            $.ajax({
                url: this.action,
                type: this.method,
                data: $(this).serialize(),
                success: function (result) {
                    loadAnswers();
                    numberofanswers++;
                    oldnumberofanswers++;
                    $('#answersheader').show();
                    CKEDITOR.instances.answer_editor.setData('');
                }
            });
        }
        return false;
    });


    //plus vote for question
    $("#questionplus").click(function () {

        var questionid = $("#questionid").val();
        //Ajax post
        $.post("/Home/VoteForQuestion", { "questionid": questionid, "vote": 1 },
            function (data) {
                
                //If the return value is text, then there was a error -> Display the error on the error-box
                if (isNaN(data)) {
                    $("#questionwarningdiv").show();
                    $("#questionwarning").text(data);
                } else {                   
                    //If data is positive, then we write the data with plus signal
                    if (data > 0) {
                        $("#questionvote").text("+" + data);
                    } else {
                        //If data is negative, than we write simply
                        $("#questionvote").text(data);
                    }
                }
            });
    });

    //negative vote for question
    $("#questionminus").click(function () {

        var questionid = $("#questionid").val();
        //ajax post
        $.post("/Home/VoteForQuestion", { "questionid": questionid, "vote": -1 },
            function (data) {
                //If the return value is text, then there was a error -> Display the error on the error-box
                if (isNaN(data)) {
                    $("#questionwarningdiv").show();
                    $("#questionwarning").text(data);
                } else {
                    //If data is positive, then we write the data with plus signal
                    if (data > 0) {
                        $("#questionvote").text("+" + data);
                    } else {
                        //If data is negative, than we write simply
                        $("#questionvote").text(data);
                    }
                }
            });
    });


    // Error-message closing
    $(".close").click(function () {
        $(".alert").hide();
    });

    //CKEDITOR
    window.onload = function () {
        CKEDITOR.replace('answer_editor');
        CKEDITOR.config.enterMode = CKEDITOR.ENTER_BR;
    };

    //Copy the content of hidden by validate
    $("#form_submit").click(function () {
        var editor_data = CKEDITOR.instances.answer_editor.getData();
        $("#ActualAnswer_Content").val(editor_data);
    }
    );

    //Enable hidden-validate
    $(function () {
        //There are more form on the page (for edit button), then using document.forms.length-1
        var validatorSettings = $.data($('form')[document.forms.length - 1], 'validator').settings;
        validatorSettings.ignore = "";

        //Validate only be submitting
        var validator = $("form").data("validator");
        if (validator) {
            validator.settings.onkeyup = false; // disable validation on keyup
            validator.settings.onclick = false;
            validator.settings.onfocusout = false;
        }
    });
});

