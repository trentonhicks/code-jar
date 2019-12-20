<template lang="pug">

    .wrapper().bg-dark
        .container

            //- Form wrapper
            .card.card-login.mx-auto
                .card-header Redeem code
                .card-body(:style="success ? {'background-image': 'url(https://media2.giphy.com/media/Ah3aXWDIt2U6a3RiEE/giphy.gif?cid=790b76118f3628efc891ae78043a3e851388b3e827d8383f&rid=giphy.gif)'} : ''")
                    
                    //- Redeem codes form
                    form(v-on:submit.prevent='RedeemCode()')
                        .form-group
                            .form-label-group
                                input#inputEmail.form-control(
                                    type='text' 
                                    pattern=".{6,}"
                                    maxlength="6"
                                    placeholder='Enter code'
                                    required='required'
                                    autofocus='autofocus'
                                    title="6 characters required"
                                    v-model="enteredCode")
                                label(for='inputEmail' class="sr-only") Enter code
                        button.btn.btn-primary.btn-block(type="submit") Redeem code

                    //- Success and failure messages display here
                    div(v-show="submitted").mt-3
                        div(v-if="success")
                            .alert.alert-success {{ successMsg }}
                        div(v-else)    
                            .alert.alert-danger {{ errorMsg }}                        
</template>

<script>
import { HTTP } from '../js/http-common.js';

module.exports = {
    name: 'Home',
    data: function() {
        return {
            enteredCode: '',
            submitted: false,
            success: false,
            successMsg: 'Code redeemed.',
            errorMsg: '',
        }
    },
    methods: {
        RedeemCode() {
            HTTP({
                method: 'post',
                url: 'redeem-code',
                data: `"${this.enteredCode}"`,
                headers: {
                    'Content-Type': 'application/json',
                }
            }).then(response => {
                this.success = true;
                this.submitted = true;
            }).catch(e => {
                this.success = false;
                this.errorMsg = 'Error! Code was not redeemed.';
                this.submitted = true;
            });
        }
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