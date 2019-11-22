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

import Code from './Code';

    module.exports = {
        data: function() {
            return {
                codes: [
                    { id: 0, text: 'af6356', expiration: 'Date', isActive: true },
                    { id: 1, text: 'bf6356', expiration: 'Date', isActive: false },
                    { id: 2, text: 'cf6356', expiration: 'Date', isActive: true },
                    { id: 3, text: 'df6356', expiration: 'Date', isActive: true },
                ],
                numberOfCodes: 0
            }
        },
        components: {
            Code
        },
        methods: {
            GenerateCodes() {
                for(var i = 0; i < this.numberOfCodes; i++)
                {
                    this.codes.push({ id: this.codes.length + 1, text: `c863b${i}`, expiration: 'Date', isActive: true });
                }
                this.numberOfCodes = 0;
            }
        }
    }

</script>

<style lang="scss" scoped>

    .card {
        margin: 30px 15px;
    }

</style>