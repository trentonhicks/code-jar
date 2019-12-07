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

                        div(v-if="success")
                            img(src="https://media2.giphy.com/media/c4rB7DMXKgktG/giphy.gif?cid=790b7611f644663d62bcffedc8e322ae952375a74900b8ee&rid=giphy.gif" style="width:100%;")
                            .alert.alert-success {{ successMsg }}
                        div(v-else)    
                            img(src="https://media1.giphy.com/media/3ornk64Apg6Ip97m5W/giphy.gif?cid=790b7611b464b33152d7ac6f7a62ee15360ec11c6a9e70d2&rid=giphy.gif" style="width:100%;")
                            .alert.alert-danger {{ errorMsg }}
                        
</template>

<script>
import { HTTP } from '../js/http-common';
import axios from 'axios';

module.exports = {
    name: 'Home',
    data: function() {
        return {
            enteredCode: '',
            submitted: false,
            success: false,
            codes: [],
            successMsg: 'Your code has been redeemed!',
            errorMsg: 'Your code sucks!',
        }
    },
    methods: {
        RedeemCode() {
            
            // Get the latest codes
            this.GetCodes();
            var index = this.codes.findIndex(code => code.stringValue == this.enteredCode);
            console.log(index);
            this.submitted = true;

            // Check if code exists
            if(index !== -1) {

                // Check if the code is active
                if(this.codes[index].state === "Active") {

                    // Redeem code
                    this.success = true;

                    // Set code to inactive
                    axios({
                        method: 'post',
                        url: 'http://localhost:5000/redeem-code',
                        data: this.codes[index].id,
                        headers: {
                            'Content-Type': 'application/json'
                        }
                    }).then(response => {
                        // Success
                    }).catch(e => {
                        // Error
                    });
                }

                // Not active
                else {
                    this.success = false;
                    this.errorMsg = 'Codes already taken!';
                }
            }

            // Doesn't exist
            else {
                this.success = false;
                this.errorMsg = "Code doesn't exist";
            }
        },
        GetCodes() {
            HTTP.get(`codes`)
            .then(response => {
                this.codes = response.data;
            })
            .catch(e => {
                this.errors.push(e)
            });
        },
    },
    created() {
        this.GetCodes();
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