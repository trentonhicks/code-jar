<template lang="pug">
    
    .card.mb-3
        .card-header.d-flex.justify-content-between.align-items-start

            .project-name
                i.fas.fa-table
                | Random Codes

            //- Generate codes
            form.form-inline(v-on:submit.prevent='GenerateCodes()')
                label.sr-only(for='numberOfCodes') Number of codes
                input#numberOfCodes.form-control.form-control-sm.mr-sm-2.ml-auto(v-model='numberOfCodes' type='number' min="0" max="1000" placeholder='Number of codes')
                button.btn.btn-sm.btn-primary(type='submit') Generate Codes

        .card-body
            .table-responsive
                table.table.table-bordered(width='100%' cellspacing='0')
                    thead
                        tr
                            th Code
                            th Status
                            th Expiration
                            th Deactivate
                    tfoot
                        tr
                            th Code
                            th Status
                            th Expiration
                            th Deactivate
                    tbody
                        Code(v-for='code in codes' :code='code' :key='code.id')

</template>

<script>

import { HTTP } from '../js/http-common';
import axios from 'axios';
import Code from './Code';

    module.exports = {
        data: function() {
            return {
                codes: [],
                numberOfCodes: 0
            }
        },
        components: {
            Code
        },
        methods: {
            GenerateCodes() {
                if(this.numberOfCodes > 0) {
                    axios({
                        method: 'post',
                        url: 'http://localhost:5000/codes',
                        data: this.numberOfCodes,
                        headers: {
                            'Content-Type': 'application/json'
                        }
                    }).then(response => {
                        this.GetCodes();
                    }).catch(e => {
                        this.errors.push(e);
                    });
                }

                this.numberOfCodes = 0;
            },
            GetCodes() {
                HTTP.get(`codes`)
                .then(response => {
                    this.codes = response.data;
                })
                .catch(e => {
                    this.errors.push(e)
                });
            }
        },
        created() {
            this.GetCodes();
        },
    }

</script>

<style lang="scss" scoped>

    .card {
        margin: 30px 15px;
    }

</style>