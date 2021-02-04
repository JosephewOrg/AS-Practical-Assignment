document.addEventListener("DOMContentLoaded", docIsReady);

function docIsReady() {
    if (document.getElementById("tb_pwd") !== null && document.getElementById("lbl_pwdchecker") != null) {
        validate();
    }
}

function validate() {
    var str = document.getElementById("tb_pwd").value;
    var checker = document.getElementById("lbl_pwdchecker");

    if (str.length < 8) {
        checker.innerHTML = "Password Length Must Be At Least 8 Characters";
        checker.style.color = "Red";
        return ("too_short");
    }
    else if (str.search(/[0-9]/) == -1) {
        checker.innerHTML = "Password Requires At Least 1 Number";
        checker.style.color = "Red";
        return ("no_number");
    }
    else if (str.search(/[a-z]/) == -1) {
        checker.innerHTML = "Password Requires At Least 1 Lowercase Character";
        checker.style.color = "Red";
        return ("no_lowercase");
    }
    else if (str.search(/[A-Z]/) == -1) {
        checker.innerHTML = "Password Requires At Least 1 Uppercase Character";
        checker.style.color = "Red";
        return ("no_uppercase");
    }
    else if (str.search(/[^A-Za-z0-9]/) == -1) {
        checker.innerHTML = "Password Requires At Least 1 Special Character";
        checker.style.color = "Red";
        return ("no_specialchar");
    }

    checker.innerHTML = "Excellent";
    checker.style.color = "Blue";
}