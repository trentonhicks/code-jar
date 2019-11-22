<template lang="pug">

.wrapper.bg-dark
    .container
        .card.card-login.mx-auto
            .card-header Login
            .card-body
                form
                    .form-group
                        .form-label-group
                            input#inputEmail.form-control(type='email' placeholder='Email address' required='required' autofocus='autofocus')
                            label(for='inputEmail' class="sr-only") Email address
                    .form-group
                        .form-label-group
                            input#inputPassword.form-control(type='password' placeholder='Password' required='required')
                            label(for='inputPassword' class="sr-only") Password
                    
                    button.btn.btn-primary.btn-block(type="submit") Login

</template>

<script>

module.exports = {
    name: 'Login',
    data: function() {
        return {}
    },
}

</script>

<style lang="scss" scoped>
    
    .wrapper {
        height: 100vh;
        padding-top: 50px;
    }

    .card-login {
        max-width: 400px;

        .form-control {
            padding: 22px 12px;
        }
    }

</style>