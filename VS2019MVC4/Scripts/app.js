

// prevent duplicate form submissions
$('form').submit(function () {    
    var btn = $(this).find(':submit');
    var btnText = btn.text();
    btn.prop('disabled', true);
    btn.html('<span class=\"spinner-border spinner-border-sm\" role=\"status\" aria-hidden=\"true\"><\/span>');
    setTimeout(function () {
        btn.text(btnText);
        btn.prop('disabled', false);
    }, 10000);
});