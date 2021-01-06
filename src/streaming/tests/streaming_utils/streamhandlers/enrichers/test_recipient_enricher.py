# Copyright 2020 Energinet DataHub A/S
#
# Licensed under the Apache License, Version 2.0 (the "License2");
# you may not use this file except in compliance with the License.
# You may obtain a copy of the License at
#
#     http://www.apache.org/licenses/LICENSE-2.0
#
# Unless required by applicable law or agreed to in writing, software
# distributed under the License is distributed on an "AS IS" BASIS,
# WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
# See the License for the specific language governing permissions and
# limitations under the License.

from pyspark.sql.dataframe import DataFrame
import pytest

from geh_stream.streaming_utils.streamhandlers.enrichers.recipient_enricher import find_sts_recipient, \
    find_ddq_recipient, find_ez_recipient, find_ddm_recipients, enrich_recipients
from geh_stream.codelists.market_evaluation_point_type import MarketEvaluationPointType


@pytest.mark.parametrize(
    "market_evaluation_point_type, quantity, expected",
    [
        pytest.param(MarketEvaluationPointType.production.name,
                     1.0,
                     "5790001330584",
                     id="E18 Production is valid"),
        pytest.param(MarketEvaluationPointType.consumption.name,
                     1.0,
                     None,
                     id="E17 Consumption is not valid"),
        pytest.param(MarketEvaluationPointType.exchange.name,
                     1.0,
                     None,
                     id="E20 Exhange is not valid"),
        pytest.param(MarketEvaluationPointType.ve_production.name,
                     1.0,
                     "5790001330584", id="D01 VE Production is valid"),
        pytest.param(MarketEvaluationPointType.surplus_production_group.name,
                     1.0,
                     "5790001330584",
                     id="D04 Surplus production group is valid")
    ],
)
def test_find_sts_recipient(market_evaluation_point_type,
                            quantity, expected,
                            enriched_data_factory):
    data = enriched_data_factory(quantity=quantity,
                                 market_evaluation_point_type=market_evaluation_point_type)
    result = find_sts_recipient(data)
    assert result.first()["intermediate_sts_recipient"] == expected


@pytest.mark.parametrize(
    "market_evaluation_point_type, energy_supplier_market_participant_mrid, expected_mrid",
    [
        pytest.param(MarketEvaluationPointType.consumption.name,
                     "8100000000108",
                     "8100000000108",
                     id="MarketEvaluationPointType is E17 consumption, so expect DDQ to be recipient"),
        pytest.param(MarketEvaluationPointType.production.name,
                     "8100000000108",
                     "8100000000108",
                     id="MarketEvaluationPointType is E18 production, so expect DDQ to be recipient"),
        pytest.param(MarketEvaluationPointType.consumption.name,
                     None,
                     None,
                     id="EnergySupplier_MarketParticipant_mRID is empty, so no recipient added")
    ],
)
def test_enrich_ddq_recipients(market_evaluation_point_type,
                               energy_supplier_market_participant_mrid,
                               expected_mrid, enriched_data_factory):
    data = enriched_data_factory(quantity=1.0,
                                 market_evaluation_point_type=market_evaluation_point_type,
                                 marketparticipant_mrid=energy_supplier_market_participant_mrid)
    result = find_ddq_recipient(data)
    assert result.first()["intermediate_ddq_recipient"] == expected_mrid


@pytest.mark.parametrize(
    "market_evaluation_point_type, domain_mrid, in_area_mrid, in_area_owner_mrid, \
        out_area_mrid, out_area_owner_mrid, expected_in_mrid, expected_out_mrid",
    [
        pytest.param(MarketEvaluationPointType.consumption.name,
                     "100000", "100000", "5790001330000", "100001", "5780001330999",
                     None, None,
                     id="MarketEvaluationPointType is E17 consumption, so no result expected"),
        pytest.param(MarketEvaluationPointType.production.name,
                     "100000", "100000", "5790001330000", "100001", "5780001330999",
                     None, None,
                     id="MarketEvaluationPointType is E18 production, so no result expected"),
        pytest.param(MarketEvaluationPointType.exchange.name,
                     "100000", "100000", "5790001330000", "100001", "5780001330999",
                     None, "5780001330999",
                     id="Expect 5780001330999 as a recipient as outbound grid area is different from domain"),
        pytest.param(MarketEvaluationPointType.exchange.name,
                     "100000", "100001", "5780001330999", "100000", "5790001330000",
                     "5780001330999", None,
                     id="Expect 5780001330999 as a recipient as inbound grid area is different from domain"),
        pytest.param(MarketEvaluationPointType.exchange.name,
                     "100000", "100001", "5780001330999", "100002", "5890002220222",
                     "5780001330999", "5890002220222",
                     id="Expect 5780001330999 and 5890002220222 as both differ from domain mrid as a recipient"),
    ],
)
def test_enrich_ddm(market_evaluation_point_type, domain_mrid, in_area_mrid,
                    in_area_owner_mrid, out_area_mrid, out_area_owner_mrid,
                    expected_in_mrid, expected_out_mrid, enriched_data_factory):
    data = enriched_data_factory(quantity=1.0,
                                 market_evaluation_point_type=market_evaluation_point_type,
                                 meteringgridarea_domain_mrid=domain_mrid,
                                 inmeteringgridarea_domain_mrid=in_area_mrid,
                                 inmeteringgridownerarea_domain_mrid=in_area_owner_mrid,
                                 outmeteringgridarea_domain_mrid=out_area_mrid,
                                 outmeteringgridownerarea_domain_mrid=out_area_owner_mrid)
    result = find_ddm_recipients(data)
    assert result.first()["intermediate_ddm_in_recipient"] == expected_in_mrid
    assert result.first()["intermediate_ddm_out_recipient"] == expected_out_mrid


@pytest.mark.parametrize(
    "market_evaluation_point_type, expected_mrid",
    [
        pytest.param(MarketEvaluationPointType.consumption.name, None,
                     id="MarketEvaluationPointType is E17 consumption, EZ should not be added as recipient"),
        pytest.param(MarketEvaluationPointType.ve_production.name, None,
                     id="MarketEvaluationPointType is D01 VE Production, EZ should not be added as recipient"),
        pytest.param(MarketEvaluationPointType.analysis.name,
                     "5790000432752", id="MarketEvaluationPointType is D02 Analysis, EZ should be added as recipient"),
        pytest.param(MarketEvaluationPointType.exchange.name, None,
                     id="MarketEvaluationPointType is E20 exchange, EZ should not be added as recipient"),
    ],
)
def test_enrich_ez_recipients_should_not_receive_consumption(market_evaluation_point_type,
                                                             expected_mrid,
                                                             enriched_data_factory):
    data = enriched_data_factory(quantity=1.0,
                                 market_evaluation_point_type=market_evaluation_point_type)
    result = find_ez_recipient(data)
    assert result.first()["intermediate_ez_recipient"] == expected_mrid


@pytest.mark.parametrize(
    "market_evaluation_point_type, expected_recipient_count",
    [
        pytest.param(MarketEvaluationPointType.consumption.name, 1,
                     id="Testing get_recipient method with simple data"),
    ],
)
def test_get_recipients(market_evaluation_point_type,
                        expected_recipient_count,
                        enriched_data_factory):
    data = enriched_data_factory(quantity=1.0,
                                 market_evaluation_point_type=market_evaluation_point_type)
    result = enrich_recipients(data)
    assert len(result.first()["RecipientList"]) == expected_recipient_count
