import pytest
from geh_stream.codelists import MarketEvaluationPointType, SettlementMethod
from geh_stream.validation.rules.vr_611 import validate_vr_611


@pytest.mark.parametrize(
    "quantity,market_evaluation_point_type,settlement_method,expected",
    [
        pytest.param(
            1E5 - 1,
            MarketEvaluationPointType.consumption.value,
            SettlementMethod.non_profiled.value,
            True,
            id="valid because production limit is not exceeded"
        ),
        pytest.param(
            1E5,
            MarketEvaluationPointType.consumption.value,
            SettlementMethod.non_profiled.value,
            False,
            id="invalid because production limit is exceeded"
        ),
        pytest.param(
            1E5,
            MarketEvaluationPointType.production.value,
            SettlementMethod.non_profiled.value,
            True,
            id="valid when exceeding limit because it's not a consumption metering point"
        ),
        pytest.param(
            1E5,
            MarketEvaluationPointType.consumption.value,
            SettlementMethod.flex_settled.value,
            True,
            id="valid when exceeding limit because it's not a non-profiled metering point"
        ),
    ],
)
def test_vr_611(quantity, market_evaluation_point_type, settlement_method, expected, enriched_data_factory):
    data = enriched_data_factory(quantity=quantity,
                                 market_evaluation_point_type=market_evaluation_point_type,
                                 settlement_method=settlement_method)
    validated_data = validate_vr_611(data)
    assert validated_data.first()["VR-611-Is-Valid"] == expected
