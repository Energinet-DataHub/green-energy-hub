import pytest
from geh_stream.codelists import MarketEvaluationPointType
from geh_stream.validation.rules.vr_251 import validate_vr_251


@pytest.mark.parametrize(
    "quantity,market_evaluation_point_type,expected",
    [
        pytest.param(
            1E6 - 1, MarketEvaluationPointType.exchange.value, True, id="valid because production limit is not exceeded"
        ),
        pytest.param(
            1E6, MarketEvaluationPointType.exchange.value, False, id="invalid because production limit is exceeded"
        ),
        pytest.param(
            1E6, MarketEvaluationPointType.production.value, True, id="valid when exceeding limit because it's not an exhange metering point"
        ),
    ],
)
def test_vr_251(quantity, market_evaluation_point_type, expected, enriched_data_factory):
    data = enriched_data_factory(quantity=quantity, market_evaluation_point_type=market_evaluation_point_type)
    validated_data = validate_vr_251(data)
    assert validated_data.first()["VR-251-Is-Valid"] == expected
