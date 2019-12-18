<template lang="pug">

    .card.mb-3
        .card-header.d-flex.justify-content-between.align-items-start

            .project-name
                i.fa.fa-table
                |  Random Codes
          
        .card-body
            .single-batch(v-if="batchSelected")
                button.mb-2.btn.btn-sm.btn-outline-secondary(@click="BatchList()")
                    i.fa.fa-angle-left
                    |  Batch list
                code-table

            //- Show batch list when batch isn't selected
            .batch-list(v-if="!batchSelected")
                form.form-inline(v-on:submit.prevent='GenerateCodes()')
                    input#batchName.form-control.form-control-sm.mr-sm-2.mb-2.mb-sm-0(type='text' placeholder='Batch name')
                    input#startDate.form-control.form-control-sm.mr-sm-2.mb-2.mb-sm-0(type='text' onfocus="(this.type='date')" placeholder='Start Date')
                    input#endDate.form-control.form-control-sm.mr-sm-2.mb-2.mb-sm-0(type='text' onfocus="(this.type='date')" placeholder='End Date')
                    input#numberOfCodes.form-control.form-control-sm.mr-sm-2.mb-2.mb-sm-0(v-model='numberOfCodes' type='number' min="0" max="1000" placeholder='Number of codes')
                    button.btn.btn-sm.btn-primary(type='submit') Create Batch

                .card(class="m-0 mt-3" v-for="batch in batchList" :key="batch.id" :disabled="batch.state == 'Inactive'")
                    .card-body
                        .row
                            .col-md
                                h5.card-title.mb-1.mb-md-3 {{ batch.name }}
                                span.badge.badge-primary.mr-2 52 codes
                                span.badge.badge-secondary Expires on 12/17/19
                            .col-md.mt-3.mt-md-0
                                button.btn.btn-sm.btn-block.btn-outline-primary(@click="ViewBatch()") View Codes
                                button.btn.btn-sm.btn-block.btn-outline-danger() Deactivate

</template>

<script>

import CodeTable from '../components/CodeTable';
import { HTTP } from '../js/http-common';

module.exports = {
    name: 'Admin',
    data: function() {
        return {
            numberOfCodes: 0,
            batchSelected: false,
            batchList: [
                {
                    id: 1,
                    name: "Batch Name",
                    state: "Active"
                },
                {
                    id: 2,
                    name: "Batch Name",
                    state: "Active"
                },
                {
                    id: 3,
                    name: "Batch Name",
                    state: "Active"
                },
            ]
        }
    },
    components: {
        CodeTable
    },
    methods: {
        GenerateCodes() {
            if(this.numberOfCodes > 0) {
                HTTP({
                    method: 'post',
                    url: 'codes',
                    data: this.numberOfCodes,
                    headers: {
                        'Content-Type': 'application/json'
                    }
                }).then(response => {
                    // Show new batch in the list of batches
                }).catch(e => {
                    // Unable to generate codes
                });
            }
        },
        ViewBatch() {
            this.batchSelected = true;
        },
        BatchList() {
            this.batchSelected = false;
        }
    },
}
</script>

<style lang="scss" scoped>

.card {
    margin: 30px 15px;
}

</style>