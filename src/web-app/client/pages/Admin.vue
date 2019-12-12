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
            
            //- Table where codes are displayed
            code-table(:codes="codes")

            div(class="btn-group" role="group" aria-label="Basic example")
                button(class="btn btn-secondary") Previous
                button(class="btn btn-secondary") Next

</template>

<script>

import CodeTable from '../components/CodeTable';
import { HTTP } from '../js/http-common';
import axios from 'axios';

module.exports = {
    name: 'Admin',
    data: function() {
        return {
            codes: [],
            numberOfCodes: 0,
            stringValue: '',
            state: "Select status",
            filteredCodes: [],
            pageNumber: 0,
            size: 10
        }
    },
    components: {
        CodeTable
    },
    methods: {
        GetCodes() {
            axios({
                method: 'get',
                url: 'http://localhost:5000/codes'
            }).then(response => {
                this.codes = response.data;
            }).catch(error => {
                // Unable to get codes
            });
        },
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
                    // Unable to generate codes
                });
            }
        },
    },
    created() {
        this.GetCodes();
    },
    computed: {
        searchQuery() {
            return [this.stringValue, this.state];
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