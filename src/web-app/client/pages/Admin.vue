<template lang="pug">

    .card.mb-3
        .card-header.d-flex.justify-content-between.align-items-start

            .project-name
                i.fa.fa-table
                |  Random Codes
          
        .card-body
            .single-batch(v-if="batchSelected")
                button.mb-2.btn.btn-sm.btn-outline-secondary(@click="GoBack()")
                    i.fa.fa-angle-left
                    |  Batch list
                code-table(:batchID="batchIDSelected")

            //- Show batch list when batch isn't selected
            .batch-list(v-if="!batchSelected")
                form.form-inline(v-on:submit.prevent='CreateBatch()')
                    
                    //- Batch Name
                    input#batchName.form-control.form-control-sm.mr-sm-2.mb-2.mb-sm-0(
                        v-model="batchName"
                        type='text'
                        placeholder='Batch name'
                        required)

                    //- Date Active
                    input#dateActive.form-control.form-control-sm.mr-sm-2.mb-2.mb-sm-0(
                        v-model="dateActive" 
                        type='text' onfocus="(this.type='date')" 
                        placeholder='Start Date' 
                        required)

                    //- Date Expires
                    input#dateExpires.form-control.form-control-sm.mr-sm-2.mb-2.mb-sm-0(
                        v-model="dateExpires" 
                        type='text' 
                        onfocus="(this.type='date')" 
                        placeholder='End Date')
                    
                    //- Batch Size
                    input#batchSize.form-control.form-control-sm.mr-sm-2.mb-2.mb-sm-0(
                        v-model='batchSize' 
                        type='number' min="0"
                        max="1000" 
                        placeholder='Number of codes')

                    //- Create Batch
                    button.btn.btn-sm.btn-primary(type='submit') Create Batch

                //- Batch list
                .card(
                    class="m-0 mt-3" 
                    v-for="batch in batches" 
                    :key="batch.id" 
                    :disabled="batch.state == 'Inactive'")
                    .card-body
                        .row
                            .col-md
                                h5.card-title.mb-1.mb-md-3 {{ batch.batchName }}
                                span.badge.badge-primary.mr-2 {{ batch.batchSize }} codes
                                span.badge.badge-secondary Expires on {{ batch.dateExpires | formatDate }}
                            .col-md.mt-3.mt-md-0
                                button.btn.btn-sm.btn-block.btn-outline-primary(@click="ViewBatch(batch.id)") View Codes
                                button.btn.btn-sm.btn-block.btn-outline-danger() Deactivate

                //- Alert when there are no batches
                p.text-muted.mt-3.mb-0(v-if="batches.length == 0" role="alert") No batches have been created yet.

</template>

<script>

import CodeTable from '../components/CodeTable';
import { HTTP } from '../js/http-common';

module.exports = {
    name: 'Admin',
    data: function() {
        return {
            batchSelected: false,
            batchIDSelected: '',
            batchName: '',
            batchSize: 0,
            dateActive: '',
            dateExpires: '',
            batches: []
        }
    },
    components: {
        CodeTable
    },
    methods: {
        CreateBatch() {
            if(this.batchSize > 0) {
                HTTP({
                    method: 'post',
                    url: 'batch',
                    data: {
                        batchName: this.batchName,
                        batchSize: parseInt(this.batchSize),
                        dateActive: this.dateActive,
                        dateExpires: this.dateExpires,
                    },
                    headers: {
                        'Content-Type': 'application/json'
                    }
                }).then(response => {
                    this.GetBatches();
                }).catch(e => {
                    // Unable to create batch
                });
            }
        },
        GetBatches() {
            HTTP({
                method: 'get',
                url: 'batch'
            }).then(response => {
                this.batches = response.data;
            });
        },
        ViewBatch(id) {
            this.batchSelected = true;
            this.batchIDSelected = id;
        },
        GoBack() {
            this.batchSelected = false;
        }
    },
    created() {
        this.GetBatches();
    },
    filters: {
        formatDate: function(dateString) {
            var date = new Date(dateString);
            return date.getMonth()+1 + "/" + date.getDate() + "/" + date.getFullYear();
        }
    },
}
</script>

<style lang="scss" scoped>

.card {
    margin: 30px 15px;
}

</style>