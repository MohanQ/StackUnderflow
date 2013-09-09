
//CKEDITOR
window.onload = function () {
    CKEDITOR.replace('edit_editor');
    CKEDITOR.config.enterMode = CKEDITOR.ENTER_BR;
    var data= $("#Content").val();
    CKEDITOR.instances.edit_editor.setData(data);
    
};

//Copy the content of hidden by validate
$("#form_submit").click(function () {
    var editor_data = CKEDITOR.instances.edit_editor.getData();
    $("#Content").val(editor_data);
}
);

//Enable hidden-validate
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


