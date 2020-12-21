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
from .vr_200 import validate_vr_200
from .vr_245_1 import validate_vr_245_1
from .vr_250 import validate_vr_250
from .vr_251 import validate_vr_251
from .vr_611 import validate_vr_611
from .vr_612 import validate_vr_612

rules = [
    validate_vr_200,
    validate_vr_245_1,
    validate_vr_250,
    validate_vr_251,
    validate_vr_611,
    validate_vr_612]
