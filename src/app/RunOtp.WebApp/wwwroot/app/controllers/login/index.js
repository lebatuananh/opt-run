let loginController = function () {
    this.initialize = function () {
        registerEvents();
    }

    let registerEvents = function () {
        $('#frmLogin').validate({
            errorClass: 'red',
            ignore: [],
            lang: 'vi',
            rules: {
                userName: {
                    required: true
                },
                password: {
                    required: true
                }
            }
        });
        $('#btnLogin').on('click', function (e) {
            if ($('#frmLogin').valid()) {
                e.preventDefault();
                let user = $('#txtUserName').val();
                let password = $('#txtPassword').val();
                login(user, password);
            }

        });
    }

    let login = function (user, pass) {
        $.ajax({
            type: 'POST',
            data: {
                Email: user,
                Password: pass
            },
            dateType: 'json',
            url: '/login/authen',
            success: function (res) {
                if (res.success) {
                    window.location.href = "/Home/Index";
                } else {
                    app.notify('Đăng nhập thất bại', 'error');
                }
            }
        })
    }
};