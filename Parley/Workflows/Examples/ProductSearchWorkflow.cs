using Microsoft.Agents.AI;
using Microsoft.Extensions.AI;
using OpenAI.Chat;
using Parley.Providers;
using System.ComponentModel;
using System.Text.Json.Serialization;

namespace Parley.Workflows.Examples;

public class ProductSearchWorkflow
{
    private readonly IAgentSchemaProvider _schemaProvider;

    public ProductSearchWorkflow(IAgentSchemaProvider schemaProvider)
    {
        _schemaProvider = schemaProvider;
    }

    public async Task<AIAgent> ConstructProductSearchWorkflow(IChatClient chatClient)
    {
        var agentSchema = await _schemaProvider.Provide();

        var baseAgent = chatClient.AsAIAgent(name: agentSchema.Name,
                                             instructions: agentSchema.Instructions,
                                             tools: [AIFunctionFactory.Create(ProductSearch),
                                                     AIFunctionFactory.Create(AddItemToBasket),
                                                     AIFunctionFactory.Create(ItemAction)]);

        return baseAgent;
    }

    [Description("When the user wants to make a change to an item already in the basket.")]
    private static ItemAction ItemAction([Description("The sku code of the item the user wants to perform an action on.")] string sku,
                                         [Description("The quantity value that the user wants to set for the item.")] int quantity,
                                         [Description("The type of action the user wants to perform, 0 or none by default.")] BasketActionType type)
    {
        return new ItemAction
        {
            Sku = sku,
            Quantity = quantity,
            Type = (int)type
        };
    }


    [Description("When the user wants to add an item to the basket. The item must NOT already be in the basket.")]
    private static StoreAction AddItemToBasket([Description("The sku code of the item the user wants to add to the basket")] string skuCode)
    {
        return new StoreAction
        {
            ActionName = "AddToBasket",
            ActionContent = skuCode
        };
    }

    [Description("""
                 When the user wants to perform a search for a product.
                 """)]
    private static SearchResult ProductSearch([Description("What the user is searching for/looking to buy.")] string searchTerm)

    {
        return new SearchResult
        {
            Products = [
                new SearchProduct {
                    Name = "Arezzo Curved Wall Hung Cloakroom Basin 400 x 220mm - Gloss White, 1 Tap Hole Left Hand",
                    Sku = "AZ78204L",
                    ImageUrl = "https://images.victorianplumbing.co.uk/products/arezzo-400-x-215mm-curved-wall-hung-1th-cloakroom-basin/variants/az78204l/mainimages/az78204lnew.webp?origin=az78204lnew.jpg&w=400",
                    ProductInformation = """
                                         The space-saving Arezzo curved wall hung cloakroom basin, a high quality curved ceramic basin with a single tap hole.
                                         Provides you with a handy space-saving solution for your cloakroom or en-suite.
                                         Some small dimensional variation may occur due to manufacturing tolerances.
                                         Waste not included.
                                         Basin comes with the option of a left or right-hand tap hole.
                                         """,
                    Details = [
                        "Size: Width: 400mm, Depth: 220mm, Height: 105mm",
                        "Bowl Depth: 90mm",
                        "Product Type: Wall Hung Basin",
                        "Shape: Curved",
                        "Material: Ceramic",
                        "Colour/Finish: Gloss White",
                        "Clean elegant contemporary design",
                        "Tap Holes: 1",
                        "No overflow",
                        "For use with a unslotted waste"
                    ]
                },
                new SearchProduct {
                    Name = "Arezzo Thin Edge Wall Hung Cloakroom Basin (400mm Wide - Gloss White)",
                    Sku = "AZ774GW",
                    ImageUrl = "https://images.victorianplumbing.co.uk/products/arezzo-thin-edge-wall-hung-cloakroom-basin-400mm-wide-gloss-white/mainimages/az774gw_l3.webp?origin=az774gw_l3.png&w=400",
                    ProductInformation = """
                                         The Arezzo wall hung basin combines sleek design with space-saving practicality.
                                         Its thin edge profile and glossy white finish add a clean, modern touch to any bathroom.
                                         Made from high-quality materials for lasting durability, it features a single tap hole and compact dimensions—ideal for smaller bathrooms or cloakrooms.
                                         Easy to clean and effortlessly stylish, it’s a smart choice for contemporary spaces.
                                         """,
                    Details = [
                        "Width: 400mm",
                        "Depth: 350mm",
                        "Product Type: Wall Hung Basin",
                        "Material: High quality vitreous china",
                        "Colour/Finish: Gloss White",
                        "Basin Shape: Soft-Square with thin edges",
                        "Style: Modern",
                        "Tap Holes: 1",
                        "Modern basin with tap ledge and overflow",
                        "High-quality construction: The basin is made from durable materials, ensuring long-lasting performance and durability."

                    ]
                },
                new SearchProduct {
                    Name = "Chatsworth Traditional Corner Cloakroom Basin 1TH - Gloss White",
                    Sku = "CHT712GW",
                    ImageUrl = "https://images.victorianplumbing.co.uk/products/chatsworth-traditional-corner-cloakroom-basin-1th-gloss-white/mainimages/cht712gwlrg.webp?origin=cht712gwlrg.jpg&w=400",
                    ProductInformation = """
                                         Maximise space in a small bathroom with the wall-mounted Chatsworth traditional corner cloakroom basin.
                                         With its compact width, it slots easily into tight spaces.
                                         This small corner sink is made from high-quality and easy-to-clean gloss ceramic.
                                         It has one tap hole for adding a space-saving mixer tap and is finished with a classic angular design.
                                         Some small dimensional variations may occur due to manufacturing tolerances.
                                         Waste is not included.
                                         """,
                    Details = [
                        "Size: Width: 550mm, Depth: 385mm, Height: 170mm",
                        "Product Type: Wall Hung Corner Basin with Tap Ledge",
                        "Material: Ceramic",
                        "Colour/Finish: Gloss White",
                        "Style: Traditional",
                        "Tap Holes: 1",
                        "Chrome ABS Overflow ",
                        "Space-Saving",
                        "Classic angular design",
                        "Ideal for Cloakrooms or en-suites"
                    ]
                },
                new SearchProduct {
                    Name = "Arezzo Gloss White Round Countertop Basin - 300mm Diameter",
                    Sku = "AZ110GW",
                    ImageUrl = "https://images.victorianplumbing.co.uk/products/arezzo-gloss-white-round-countertop-basin-300mm-diameter/mainimages/az110gw_l2.webp?origin=az110gw_l2.png&w=400",
                    ProductInformation = """
                                         The Arezzo Gloss White Round Countertop Basin is a stylish addition to any bathroom.
                                         With a sleek, modern design and a diameter of 300mm, this basin is perfect for smaller bathrooms or for those who prefer a minimalist aesthetic.
                                         The glossy white finish adds a touch of elegance to the basin, while the round shape provides a soft and inviting look.
                                         This countertop basin is made from high-quality materials, ensuring durability and longevity.
                                         It is easy to install and maintain, making it a practical and stylish choice for any bathroom.
                                         Upgrade your bathroom today with the Arezzo Gloss White Round Countertop Basin.
                                         """,
                    Details = [
                        "Shape: Round",
                        "Colour: White",
                        "Finish: Gloss",
                        "Material: Ceramic",
                        "Design: Clean, elegant, contemporary"
                    ]
                },
                new SearchProduct {
                    Name = "Chatsworth 535 x 390mm Traditional Oval Countertop Basin - Gloss White",
                    Sku = "CH535CT",
                    ImageUrl = "https://images.victorianplumbing.co.uk/products/chatsworth-535-x-390-mm-traditional-oval-countertop-basin-gloss-white/mainimages/ch535ct_l4.webp?origin=ch535ct_l4.png&w=400",
                    ProductInformation = """
                                         Introducing the Chatsworth 535 x 390 Oval Rolltop Style Traditional Countertop Basin, a stunning addition to any bathroom.
                                         Its unique oval shape and classic rolltop design exude elegance and sophistication.
                                         Crafted with high-quality materials, this basin ensures long-lasting durability and performance.
                                         The smooth surface makes cleaning a breeze, keeping it pristine with minimal effort.
                                         Versatile installation options allow you to mount it on a countertop or a vanity unit, adapting to your preferred style and layout.
                                         With its timeless design and attention to detail, this basin complements any traditional bathroom decor, adding a touch of luxury and sophistication.
                                         """,
                    Details = [
                        "Width: 535mm",
                        "Depth: 390mm",
                        "Height: 160mm",
                        "Range: Chatsworth",
                        "Style: Traditional",
                        "Material: Ceramic",
                        "Tap Holes: 0",
                        "Type: Countertop Basin",
                        "Shape: Oval",
                        "Oval rolltop design",
                    ]
                }
            ]
        };
    }
}

public class SearchResult
{
    public ICollection<SearchProduct> Products { get; set; } = new List<SearchProduct>();
}

public class SearchProduct
{
    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;

    [JsonPropertyName("sku")]
    public string Sku { get; set; } = string.Empty;

    [JsonPropertyName("imageUrl")]
    public string ImageUrl { get; set; } = string.Empty;

    [JsonPropertyName("productInformation")]
    public string ProductInformation { get; set; } = string.Empty;

    [JsonPropertyName("details")]
    public ICollection<string> Details { get; set; } = [];
}

public class StoreAction
{
    public string ActionName { get; set; } = string.Empty;
    public string ActionContent { get; set; } = string.Empty;
}

[Description("A list of actions that the user wants to perform on the basket and/or the items in it.")]
public class BasketAction
{
    public List<ItemAction> ItemActions = [];
}

[Description("An action that the user wants to perform on a specific item in the basket.")]
public class ItemAction
{
    [Description("The sku code of the item the user wants to perform an action on.")]
    public string Sku { get; set; } = string.Empty;
    [Description("The quantity value that the user wants to set for the item.")]
    public int Quantity { get; set; }
    [Description("The type of action the user wants to perform, 0 or none by default.")]
    public int Type { get; set; }
}

public enum BasketActionType
{
    None,
    SetQuantity,
    Remove
}