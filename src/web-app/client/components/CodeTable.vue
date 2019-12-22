<template lang="pug">

    div
        form#searchCodes.form-inline
            label.sr-only(for='searchCode') Search for Codes 
            input#searchCode.form-control.form-control-sm.mr-sm-2(v-model='stringValue' type='text' placeholder='Search codes') 
        
            select#searchStatuses.custom-select(v-model='state')
                option(selected) Select status
                option(value="Active") Active
                option(value="Redeemed") Redeemed 
                option(value="Inactive") Inactive

        .table-responsive
            table.table.table-bordered(width='100%' cellspacing='0')
                thead
                    tr
                        th Code
                        th Status
                        th Deactivate
                tfoot
                    tr
                        th Code
                        th Status
                        th Deactivate

                tbody
                    Code(v-for='code in codes' :code='code' :key='code.id')

        div(class="btn-group" role="group" aria-label="Pagination")
            button(class="btn btn-secondary" @click="PrevPage()" :disabled="pageNumber == 1") Previous
            button(class="btn btn-secondary" @click="NextPage()" :disabled="pageNumber == pages") Next
        span.text-secondary.ml-3 {{ pageNumber }} of {{ pages }}

</template>

<script>

import Code from './Code';
import { HTTP } from '../js/http-common';

    module.exports = {
        data: function() {
            return {
                codes: [],
                filteredCodes: [],
                state: "Select status",
                stringValue: '',
                pageNumber: 1,
                pages: 0,
                size: 10,
            }
        },
        props: ['batchID'],
        components: {
            Code
        },
        methods: {
            GetTableData(stringValue, state) {
                var params = { page: this.pageNumber };

                if(stringValue != "") {
                    params.stringValue = this.stringValue;
                }

                if(state != "Select status") {
                    params.state = this.state;
                }

                HTTP({
                    method: 'get',
                    url: `batch/${this.batchID}`,
                    params,
                }).then(response => {
                    this.codes = response.data.codes;
                    this.pages = response.data.pages;
                }).catch(error => {
                    // Unable to get codes
                });
            },
            PrevPage() {
                this.pageNumber--;
            },
            NextPage() {
                this.pageNumber++; 
            },
        },
        created() {
            this.GetTableData(this.stringValue, this.state);
        },
        computed: {
            searchQuery() {
                return [this.stringValue, this.state];
            },
        },
        watch: {
            pageNumber: function() {
                this.GetTableData(this.stringValue, this.state);
            },
            searchQuery: function() {
                this.GetTableData(this.stringValue, this.state);
            }
        },
    }

</script>

<style lang="scss" scoped>

#searchCodes {
    padding-bottom: 20px;
}

#searchStatuses.custom-select{
    font-size: .875rem;
    height: calc(1.3em + .75rem + 2px); 
}

</style>