if (document.getElementById("g-recaptcha-response") != null) {
    grecaptcha.ready(function () {
        grecaptcha.execute('', {
            action: 'Submit'
        }).then(function (token) {
            document.getElementById("g-recaptcha-response").value = token;
        });
    });
}
