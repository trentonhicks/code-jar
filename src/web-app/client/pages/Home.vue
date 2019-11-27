<template lang="pug">

    .wrapper().bg-dark
        .container
            .card.card-login.mx-auto
                .card-header Redeem code
                .card-body(:style="success ? {'background-image': 'url(https://media2.giphy.com/media/Ah3aXWDIt2U6a3RiEE/giphy.gif?cid=790b76118f3628efc891ae78043a3e851388b3e827d8383f&rid=giphy.gif)'} : ''")
                    form(v-on:submit.prevent='RedeemCode()')
                        .form-group
                            .form-label-group
                                input#inputEmail.form-control(
                                    type='text' 
                                    pattern=".{6,}"
                                    maxlength="6"
                                    placeholder='Enter 6-digit code'
                                    required='required'
                                    autofocus='autofocus'
                                    title="6 characters required"
                                    v-model="enteredCode")

                                label(for='inputEmail' class="sr-only") Enter code
                        
                        button.btn.btn-primary.btn-block(type="submit") Redeem code

                    div(v-show="submitted").mt-3
                        img(v-if="success" src="https://media2.giphy.com/media/c4rB7DMXKgktG/giphy.gif?cid=790b7611f644663d62bcffedc8e322ae952375a74900b8ee&rid=giphy.gif" style="width:100%;")
                        img(v-else src="https://media1.giphy.com/media/3ornk64Apg6Ip97m5W/giphy.gif?cid=790b7611b464b33152d7ac6f7a62ee15360ec11c6a9e70d2&rid=giphy.gif" style="width:100%;")


</template>

<script>
import { HTTP } from '../js/http-common';

module.exports = {
    name: 'Home',
    data: function() {
        return {
            enteredCode: '',
            submitted: false,
            success: false,
            codes: [],
            errors: []
        }
    },
    created() {
        HTTP.get(`codes`)
        .then(response => {
            this.codes = response.data;
        })
        .catch(e => {
            this.errors.push(e)
        })
    },
    methods: {
        RedeemCode() {
            const index = this.codes.findIndex(code => code.stringValue == this.enteredCode);
            this.submitted = true;

            // Check if code exists
            if(index !== -1) {

                // Check if the code is active
                if(this.codes[index].state === "Active") {

                    // Redeem code
                    this.success = true;

                    // Set code to inactive
                    this.$set(this.codes[index], 'state', "Inactive");
                }

                // If code isn't active, display error message
                else {
                    console.log(`Code ${this.codes[index].stringValue} is not active`);
                }
            }

            // If code doesn't exist, display error message
            else {
                console.log("Code doesn't exist");
            }
        },
    }
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