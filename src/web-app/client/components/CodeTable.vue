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
            form#searchCodes.form-inline
                label.sr-only(for='searchCode') Search for Codes 
                input#searchCode.form-control.form-control-sm.mr-sm-2(v-model='stringValue' type='text' placeholder='Search codes') 
           
                select#searchStatuses.custom-select(v-model='state')
                    option(selected) Select status
                    option(value="Active") Active
                    option(value="Redeemed") Redemeed 
                    option(value="Expired") Expired
                    option(value="Inactive") Inactives

           

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
                        Code(v-for='code in paginatedData' :code='code' :key='code.id')

                div(class="btn-group" role="group" aria-label="Basic example")
                        button(v-on:click="prevPage" :disabled="pageNumber==0" class="btn btn-secondary") Previous
                        button(v-on:click="nextPage" :disabled="pageNumber >= pageCount -1" class="btn btn-secondary") Next
               
</div>
</template>

<script>

import { HTTP } from '../js/http-common';
import axios from 'axios';
import Code from './Code';

    module.exports = {
        data: function() {
            return {
                codes: [],
                numberOfCodes: 0,
                stringValue: '',
                state: "Select status",
                filteredCodes: [],
                pageNumber: 0
            }
        },
        props:{
            size: {
                type:Number,
                required:false,
                default: 10
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
            },
            GetCodes() {
                HTTP.get(`codes`)
                .then(response => {
                    this.codes = response.data;
                    this.filteredCodes = this.codes;
                    
                })
                .catch(e => {
                    this.errors.push(e)
                });
            },
            nextPage() {
            this.pageNumber++;
            },
            prevPage(){
                this.pageNumber--;
            }
        },
        created() {
            this.GetCodes();
        },
        computed: {
            searchQuery() {
                return [this.stringValue, this.state];
            },
            pageCount(){
               let l = this.filteredCodes.length,
                   s = this.size;
               return Math.ceil(l/s);
            },
            paginatedData(){
                const start = this.pageNumber * this.size,
                    end = start + this.size;
                return this.filteredCodes.slice(start, end);
            }
        },
        watch: {
            searchQuery([stringValue, state]) {
                // Filter codes by the searched string value
                this.filteredCodes = this.codes.filter(code => code.stringValue.includes(stringValue));

                // Add an additional filter by code state if that option is selected
                if(state != 'Select status') {
                    this.filteredCodes = this.filteredCodes.filter(code => code.state == state);
                }
            },
        },
    }

</script>

<style lang="scss" scoped>

    .card {
        margin: 30px 15px;
    }

    #searchCodes {
        padding-bottom: 20px;
    }
    #searchStatuses.custom-select{
      font-size: .875rem;
      height: calc(1.3em + .75rem + 2px);
       
    }
</style>