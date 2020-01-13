<template lang="pug">

    div
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

                    tr(v-if="codes.length < 1")
                        td Unable to find codes that match your search
                        td
                        td

        div(class="btn-group" role="group" aria-label="Pagination" v-if="!searchResults")
            button(class="btn btn-secondary" @click="PrevPage()" :disabled="pageNumber == 1") Previous
            button(class="btn btn-secondary" @click="NextPage()" :disabled="pageNumber == pages") Next
        span.text-secondary.ml-3(v-if="!searchResults") {{ pageNumber }} of {{ pages }}

</template>

<script>

import Code from './Code';
import { HTTP } from '../js/http-common';

    module.exports = {
        data: function() {
            return {
                codes: [],
                filteredCodes: [],
                stringValue: '',
                pageNumber: 1,
                pages: 0,
                size: 10,
                searchResults: false,
            }
        },
        props: ['batchID', 'search'],
        components: {
            Code
        },
        methods: {
            GetTableData(search) {
                var params = {};
                var url = this.batchID != undefined ? `batch/${this.batchID}` : 'codes';

                // Add search query if user searched
                if(this.search != undefined) {
                    params.stringValue = this.search;
                }

                else {
                    params.page = this.pageNumber;
                }

                HTTP({
                    method: 'get',
                    url,
                    params,
                }).then(response => {
                    if(this.batchID != undefined) {
                        this.codes = response.data.codes;
                        this.pages = response.data.pages;
                    }
                    else if(response.data.state != null) {
                        this.codes.push(response.data);
                    }
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
            this.GetTableData(this.stringValue);

            if(this.search != undefined) {
                this.searchResults = true;
            }
        },
        computed: {
            searchQuery() {
                return [this.stringValue];
            },
        },
        watch: {
            pageNumber: function() {
                this.GetTableData(this.search);
            },
            searchQuery: function() {
                this.GetTableData(this.search);
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